﻿using SoftwareInstallationBusinessLogic.BindingModels;
using SoftwareInstallationBusinessLogic.Enums;
using SoftwareInstallationBusinessLogic.HelperModels;
using SoftwareInstallationBusinessLogic.Interfaces;
using SoftwareInstallationBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareInstallationBusinessLogic.BusinessLogic
{
    public class ReportLogic
    {
        private readonly IPackageStorage _packageStorage;
        private readonly IOrderStorage _orderStorage;
        public ReportLogic(IPackageStorage packageStorage, IOrderStorage orderStorage)
        {
            _packageStorage = packageStorage;
            _orderStorage = orderStorage;
        }

        //Получение списка компонентов с указанием того, в каких изделиях они используются
        public List<ReportPackageComponentViewModel> GetPackageComponent()
        {
            var packages = _packageStorage.GetFullList();
            var list = new List<ReportPackageComponentViewModel>();

            foreach (var package in packages)
            {
                var record = new ReportPackageComponentViewModel
                {
                    PackageName = package.PackageName,
                    PackageComponents = new List<Tuple<string, int>>(),
                    TotalCount = 0
                };

                foreach (var component in package.PackageComponents)
                {
                    record.PackageComponents.Add(new Tuple<string, int>(component.Value.Item1, component.Value.Item2));
                    record.TotalCount += component.Value.Item2;
                }
                list.Add(record);
            }
            return list;
        }

        //Получение списка заказов за определённый период
        public List<ReportOrdersViewModel> GetOrders(ReportBindingModel model)
        {
            return _orderStorage.GetFilteredList(new OrderBindingModel { DateFrom = model.DateFrom, DateTo = model.DateTo })
            .Select(x => new ReportOrdersViewModel
            {
                DateCreate = x.DateCreate,
                PackageName = x.PackageName,
                Count = x.Count,
                Sum = x.Sum,
                Status = Convert.ToString((OrderStatus)Enum.Parse(typeof(OrderStatus), x.Status.ToString()))
            })
           .ToList();
        }

        //Сохранение компонентов в Word-файл
        public void SavePackagesToWordFile(ReportBindingModel model)
        {
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = model.FileName,
                Title = "Список пакетов",
                Packages = _packageStorage.GetFullList()
            });
        }

        //Сохранение компонентов с указанием пакетов в Excel-файл
        public void SavePackageComponentToExcelFile(ReportBindingModel model)
        {
            SaveToExcel.CreateDoc(new ExcelInfo
            {
                FileName = model.FileName,
                Title = "Список компонентов по пакетам",
                PackageComponents = GetPackageComponent()
            });
        }

        //Сохранение заказов в Pdf-файл
        public void SaveOrdersToPdfFile(ReportBindingModel model)
        {
            SaveToPdf.CreateDoc(new PdfInfo
            {
                FileName = model.FileName,
                Title = "Список заказов",
                DateFrom = model.DateFrom.Value,
                DateTo = model.DateTo.Value,
                Orders = GetOrders(model)
            });
        }
    }
}