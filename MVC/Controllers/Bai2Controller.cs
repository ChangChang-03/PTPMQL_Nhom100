namespace MVC.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using MVC.Models;
    public class Bai2Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            ViewData["Message"] = "Your welcome message";

            return View();
        }
        [HttpPost]
        public IActionResult Index(Bai2 b2)
        {
            double result = 0;
            string message = "";

            switch (b2.Op)
            {
                case "sum":
                    result = b2.A + b2.B;
                    message = $"{b2.A} + {b2.B} = {result}";
                    break;

                case "diff":
                    result = b2.A - b2.B;
                    message = $"{b2.A} - {b2.B} = {result}";
                    break;

                case "mul":
                    result = b2.A * b2.B;
                    message = $"{b2.A} * {b2.B} = {result}";
                    break;

                case "div":
                    if (b2.B != 0)
                    {
                        result = b2.A / b2.B;
                        message = $"{b2.A} / {b2.B} = {result}";
                    }
                    else
                    {
                        message = "Không thể chia cho 0!";
                    }
                    break;

                default:
                    message = "Vui lòng chọn phép toán!";
                    break;
            }

            ViewBag.Message = message;
            return View();
        }
    }
}