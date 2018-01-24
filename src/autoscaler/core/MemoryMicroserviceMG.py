from .MicroserviceMonitoringGroup import MicroserviceMonitoringGroup
from . import limit_range
from . import utils
import math

class MemoryMicroserviceMG(MicroserviceMonitoringGroup):

    def __init__(self, microservice_name = 'memory_microservice', min_scale = 1, max_scale = 40, threshold =  150 * 1024.0 * 1024.0 ):
        super().__init__(microservice_name)
        self._min_scale = min_scale
        self._max_scale = max_scale
        self._scale_up_rule = utils.DelayedActionHelper()
        self._scale_down_rule = utils.DelayedActionHelper()
        self.MEMORY_THRESHOLD = threshold
        
    def _check(self):
        memTotal = self._res.GetMemoryUsage('30s')
        targetScale =  math.ceil(cpuTotal / self.MEMORY_THRESHOLD)
        targetScale = limit_range(targetScale, self._min_scale, self._max_scale)
        currentScale = float(self._swarm.GetScaleTarget())
 
        print("###MEM",memTotal, targetScale, self._scale_up_rule.activeFor().total_seconds(),self._scale_down_rule.activeFor().total_seconds(),(targetScale - currentScale) / currentScale)
        if ((targetScale - currentScale) / currentScale > 0.05):
            # scale up rule
            self._scale_up_rule.setActive()
            if (self._scale_up_rule.activeFor().total_seconds() > 30):
                self._do_scale(targetScale)
                self._scale_up_rule.setInactive()
        else:
            self._scale_up_rule.setInactive()

        if ((currentScale - targetScale) / currentScale > 0.05):
            # scale down rule
            self._scale_down_rule.setActive()
            if (self._scale_down_rule.activeFor().total_seconds() > 70):
                self._do_scale(targetScale)
                self._scale_down_rule.setInactive()
        else:
            self._scale_down_rule.setInactive()
