using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;

namespace GrpcChat.Host.Controllers
{
    public class ValuesController : ControllerBase
    {
        private IHostingEnvironment hostingEnvironment;

        public ValuesController(IHostingEnvironment hostingEnvironment) => this.hostingEnvironment = hostingEnvironment;

        public IActionResult Index()
        {
            string webRootPath = hostingEnvironment.WebRootPath;
            string contentRootPath = hostingEnvironment.ContentRootPath;

            return Content(webRootPath + "\n" + contentRootPath);
        }
    }
}