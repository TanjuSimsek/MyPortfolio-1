﻿using MyPortfolio.Model;
using MyPortfolio.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MyPortfolio.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly IPageService pageService;
        private readonly IFeedbackService feedbackService;
        private readonly INewsletterService newsletterService;

        public HomeController(IPageService pageService, IFeedbackService feedbackService, INewsletterService newsletterService)
        {
            this.pageService = pageService;
            this.feedbackService = feedbackService;
            this.newsletterService = newsletterService;
        }


        public ActionResult Index()
        {
            ViewBag.user = ConfigurationManager.AppSettings["myKey"];
            return View();
        }

        public ActionResult About()
        {
            var page = pageService.FindByTitle("Hakkında");
            return View(page);
        }

        public ActionResult Portfolio()
        {

            ViewBag.Message = "Your portfolio page.";

            return View();
        }
        public ActionResult Blog()
        {

            ViewBag.Message = "Your  Blog page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "İletişim sayfası.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string firstName, string lastName, string email, string phone, string department, string message)
        {

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName == "")
            {

                ViewBag.Message = "Ad alanı gereklidir.";
                ViewBag.IsError = true;
                return View();
            }
            if (firstName.Length > 50)
            {
                ViewBag.Message = "Ad alanı 50 karakterden uzun olamaz";
                ViewBag.IsError = true;
                return View();
            }
            if (lastName == "")
            {

                ViewBag.Message = "Soyad alanı gereklidir";
                ViewBag.IsError = true;
                return View();
            }

            Regex regex = new Regex(@"^5(0[5-7]|[3-5]\d) ?\d{3} ?\d{4}$");//
            Match match = regex.Match(phone);
            if (match.Success == false)
            {
                ViewBag.Message = "Telefon 5XX XXX XXXX biçiminde olmalıdır.";
                ViewBag.IsError = true;
                return View();
            }

            var fed = new Feedback();
            fed.Name = firstName;
            fed.Email = email;
            fed.Subject = department;
            fed.Message = message;

            feedbackService.Insert(fed);
            //TODO Mail Gönderme İşlemi


            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress("tanerakyil@gmail.com", "tano");
            mailMessage.Subject = "İletişim Formu: " + firstName + " " + lastName;
            mailMessage.To.Add("tanerakyil@gmail.com,tanerakyil@gmail.com");
            string body;
            body = "Ad: " + firstName + "<br />";
            body = "Soyad: " + lastName + "<br />";
            body += "Telefon: " + phone + "<br />";
            body += "E-posta: " + email + "<br />";
            body += "Depart: " + department + "<br />";
            body += "Mesaj: " + message + "<br />";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;


            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("mail", "sifre");
            smtp.EnableSsl = true;
            smtp.Send(mailMessage);

            ViewBag.Message = "Mesajınız gönderildi. Teşekkür ederiz.";
            //TODO: Mail Gönderme işlemi yapılacak.
            ViewBag.Message = "Form başarıyla iletildi,en kısa zamanda dönüş yapacağız.";
            return View();
        }

        [HttpPost]
        public ActionResult NewsletterSubscribtion(Newsletter newsletter, string controllerName, string actionName)
        {
            if (ModelState.IsValid)
            {
                var getEmails = newsletterService.GetAll();
                if (getEmails.FirstOrDefault(f => f.Email == newsletter.Email) == null)
                {
                    newsletterService.Insert(newsletter);
                    return RedirectToAction(actionName, controllerName);
                }
                TempData["Message"] = "Bu E-Posta adresi sisteme kayıtlıdır.";
            }
            return Redirect("/" + controllerName + "/" + actionName + "/#mc_embed_signup");
        }
    }
}