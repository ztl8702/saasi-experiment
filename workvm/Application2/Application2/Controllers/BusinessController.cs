﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Application2.Controllers
{
    public class BusinessController : Controller
    {
        private Configuration ConfigSettings { get; set; }
        // GET: /<controller>/
        public BusinessController(IOptions<Configuration> settings)
        {
            ConfigSettings = settings.Value;
        }
        // GET: /<controller>/
        public IActionResult Index(int? io = 0, int? cpu = 0, int? memory = 0, int? timeout = 0)
        {
          /*  for (int i = 0; i < 4; i++)
            {
                var order = ConfigSettings.record[i].Split(' ');
                if (order[0].Equals("1"))
                {
                    cpu CPU = new cpu(order[3]);
                    new Thread(CPU.Fun).Start();

                }
                if (order[1].Equals("1"))
                {
                    io IO = new io(order[3]);
                    new Thread(IO.Fun).Start();
                }
                if (order[2].Equals("1"))
                {
                    memory MEM = new memory(order[3]);
                    new Thread(MEM.Fun).Start();
                }
            }*/
            if (cpu == 1)
            {
                cpu CPU = new cpu(Convert.ToString(timeout));
                new Thread(CPU.Fun).Start();
            }

            if (io == 1)
            {
                io IO = new io(Convert.ToString(timeout));
                new Thread(IO.Fun).Start();
            }

            if (memory == 1)
            {
                memory MEM = new memory(Convert.ToString(timeout));
                new Thread(MEM.Fun).Start();
            }
            return View();
        }
    }
}