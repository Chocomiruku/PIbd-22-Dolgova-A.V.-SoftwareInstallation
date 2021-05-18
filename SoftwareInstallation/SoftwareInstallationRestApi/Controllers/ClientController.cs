﻿using Microsoft.AspNetCore.Mvc;
using SoftwareInstallationBusinessLogic.BindingModels;
using SoftwareInstallationBusinessLogic.BusinessLogic;
using SoftwareInstallationBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SoftwareInstallationRestApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientLogic _logic;
        private readonly MailLogic _mailLogic;

        private readonly int _passwordMaxLength = 50;
        private readonly int _passwordMinLength = 10;

        private readonly int messagesOnPage = 2;

        public ClientController(ClientLogic logic, MailLogic mailLogic)
        {
            _logic = logic;
            _mailLogic = mailLogic;
            if (messagesOnPage < 1) { messagesOnPage = 5; }
        }

        [HttpGet]
        public ClientViewModel Login(string login, string password)
        {
            var client = _logic.Read(new ClientBindingModel
            {
                Email = login,
                Password = password
            });
            return (client != null && client.Count > 0) ? client[0] : null;
        }

        [HttpGet]
        public (List<MessageInfoViewModel>, bool) GetMessages(int clientId, int page)
        {
            var list = _mailLogic.Read(new MessageInfoBindingModel { ClientId = clientId, ToSkip = (page - 1) * messagesOnPage, ToTake = messagesOnPage + 1 }).ToList();
            var hasNext = !(list.Count() <= messagesOnPage);
            return (list.Take(messagesOnPage).ToList(), hasNext);
        }

        [HttpPost]
        public void Register(ClientBindingModel model)
        {
            CheckData(model);
            _logic.CreateOrUpdate(model);
        }

        [HttpPost]
        public void UpdateData(ClientBindingModel model)
        {
            CheckData(model);
            _logic.CreateOrUpdate(model);
        }

        private void CheckData(ClientBindingModel model)
        {
            if (!Regex.IsMatch(model.Email, @"^[A-Za-z0-9]+(?:[._%+-])?[A-Za-z0-9._-]+[A-Za-z0-9]@[A-Za-z0-9]+(?:[.-])?[A-Za-z0-9._-]+\.[A-Za-z]{2,6}$"))
            {
                throw new Exception("В качестве логина должна быть указана почта");
            }

            if (model.Password.Length > _passwordMaxLength || model.Password.Length < _passwordMinLength || !Regex.IsMatch(model.Password, 
                @"^((\w+\d+\W+)|(\w+\W+\d+)|(\d+\w+\W+)|(\d+\W+\w+)|(\W+\w+\d+)|(\W+\d+\w+))[\w\d\W]*$"))
            {
                throw new Exception($"Пароль должен содержать не менее {_passwordMinLength} и не более {_passwordMaxLength} символов и состоять из цифр, букв и небуквенных символов");
            }
        }
    }
}