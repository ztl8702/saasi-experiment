﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Saasi.Shared.Workload
{
    public class IoWorkload
    {
        public static readonly string randomFilePath = Directory.GetCurrentDirectory() + "\\" + "RandomStringFile" + ".txt";

        public async Task<ExecutionResult> Run(Int64 startByte, Int64 length)
        {
            var startTime = System.DateTime.Now;
            var exceptions = false;
            string result = "";
            try {
                result = await DiskIoProcess(startByte, length);
            } catch {
                exceptions = true;
            }
            
            return new ExecutionResult {
                TaskStartedAt = startTime,
                TaskFinishedAt = System.DateTime.Now,
                HasExceptions = exceptions,
                ThreadOfExecution = Thread.CurrentThread.GetHashCode().ToString(),
                Payload = result
            };
        }

        public async Task<string> DiskIoProcess(Int64 startByte, Int64 length)
        {
            
            if (File.Exists(randomFilePath) == false)
            {
                return("file not found: " + randomFilePath);
            }
            FileStream fs = new FileStream(randomFilePath, FileMode.Open, FileAccess.Read, FileShare.None);

            Int64 fileSize = fs.Length;
            if (startByte > fileSize)
            {
				fs.Dispose();
                return ("startByte is too big");
            }
            fs.Seek(startByte, SeekOrigin.Begin);
          
            
            int maxInt = int.MaxValue;
            byte[] readBuffer ;
            int numRead;
            Int64 numReadTotal = 0;

            StringBuilder sb = new StringBuilder();

            if ((startByte + length) > fileSize)
            {
                if ((fileSize - startByte) <= maxInt)
                {
                    readBuffer = new byte[fileSize - startByte];
                    numRead = await fs.ReadAsync(readBuffer, 0, (int)(fileSize - startByte));
                    string text = Encoding.Unicode.GetString(readBuffer, 0, numRead);
                    sb.Append(text);
                }
                else
                {
                    readBuffer = new byte[maxInt];
                    while (((numRead = await fs.ReadAsync(readBuffer, 0, maxInt)) != 0)&&(numReadTotal+numRead) <= (fileSize - startByte))
                    {
                        string text = Encoding.Unicode.GetString(readBuffer, 0, numRead);
                        sb.Append(text);
                        numReadTotal += numRead;
                    }
                    if (numReadTotal == (fileSize - startByte))
                    {
                        //do nothing
                    }
                    else
                    {
                        fs.Seek((numReadTotal+startByte), SeekOrigin.Begin);
                        await fs.ReadAsync(readBuffer, 0, (int)(fileSize - startByte - numReadTotal));
                        string text2 = Encoding.Unicode.GetString(readBuffer, 0, numRead);
                        sb.Append(text2);
                    }
                    
                }
            }
            else
            {
                if (length <= maxInt)
                {
                    readBuffer = new byte[length];
                    numRead = await fs.ReadAsync(readBuffer, 0, (int)length);
                    string text = Encoding.Unicode.GetString(readBuffer, 0, numRead);
                    sb.Append(text);
                }
                else
                {
                    readBuffer = new byte[maxInt];
                    while (((numRead = await fs.ReadAsync(readBuffer, 0, maxInt)) != 0)&& (numReadTotal+numReadTotal) <=length)
                    {
                        string text = Encoding.Unicode.GetString(readBuffer, 0, numRead);
                        sb.Append(text);
                        numReadTotal += numRead;
                    }
                    if (numReadTotal == length)
                    {
                        //do nothing
                    }
                    else
                    {
                        fs.Seek((numReadTotal + startByte), SeekOrigin.Begin);
                        await fs.ReadAsync(readBuffer, 0, (int)(length - numReadTotal));
                        string text2 = Encoding.Unicode.GetString(readBuffer, 0, numRead);
                        sb.Append(text2);

                    }
                }
            }
			fs.Dispose();
			fs.Close();

            return sb.ToString();

        }

        public static string GenerateRandomStringFile(Int64 length)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            var sb = new StringBuilder();
            for (Int64 i = 0; i < length; i++)
            {
                int c = r.Next(97, 123);
                sb.Append(Char.ConvertFromUtf32(c));
            }
            //If not exist, create a random string file
            string randomFilePath = Directory.GetCurrentDirectory() + "\\" + "RandomStringFile" + ".txt";
            if (File.Exists(randomFilePath))
                File.Delete(randomFilePath);
            FileStream fs = new FileStream(randomFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            // if it is needed to be asynchronously?
            sw.Write(sb);

            fs.Flush();
            sw.Dispose();
            fs.Dispose();
            return "successfully writen";
        }
    }
}
