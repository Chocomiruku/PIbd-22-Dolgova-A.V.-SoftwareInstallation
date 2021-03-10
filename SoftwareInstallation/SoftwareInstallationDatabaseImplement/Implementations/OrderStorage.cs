﻿using SoftwareInstallationBusinessLogic.BindingModels;
using SoftwareInstallationBusinessLogic.Interfaces;
using SoftwareInstallationBusinessLogic.ViewModels;
using SoftwareInstallationDatabaseImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareInstallationDatabaseImplement.Implementations
{
    public class OrderStorage : IOrderStorage
    {
        public List<OrderViewModel> GetFullList()
        {
            using (var context = new SoftwareInstallationDatabase())
            {
                return context.Orders
                .Select(rec => new OrderViewModel
                {
                    Id = rec.Id,
                    PackageId = rec.PackageId,
                    PackageName = context.Packages.FirstOrDefault(recPC => recPC.Id == rec.PackageId).PackageName,
                    Count = rec.Count,
                    Sum = rec.Sum,
                    Status = rec.Status,
                    DateCreate = rec.DateCreate,
                    DateImplement = rec.DateImplement,
                })
                .ToList();
            }
        }

        public List<OrderViewModel> GetFilteredList(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }

            using (var context = new SoftwareInstallationDatabase())
            {
                return context.Orders
                .Where(rec => rec.PackageId == model.PackageId)
                .Select(rec => new OrderViewModel
                {
                    Id = rec.Id,
                    PackageId = rec.PackageId,
                    PackageName = context.Packages.FirstOrDefault(recPC => recPC.Id == rec.PackageId).PackageName,
                    Count = rec.Count,
                    Sum = rec.Sum,
                    Status = rec.Status,
                    DateCreate = rec.DateCreate,
                    DateImplement = rec.DateImplement,
                })
                .ToList();
            }
        }

        public OrderViewModel GetElement(OrderBindingModel model)
        {
            if (model == null)
            {
                return null;
            }

            using (var context = new SoftwareInstallationDatabase())
            {
                Order order = context.Orders
                .FirstOrDefault(rec => rec.Id == model.Id);
                return order != null ?
                new OrderViewModel
                {
                    Id = order.Id,
                    PackageId = order.PackageId,
                    PackageName = context.Packages.FirstOrDefault(rec => rec.Id == order.PackageId)?.PackageName,
                    Count = order.Count,
                    Sum = order.Sum,
                    Status = order.Status,
                    DateCreate = order.DateCreate,
                    DateImplement = order.DateImplement,
                } :
                null;
            }
        }
        public void Insert(OrderBindingModel model)
        {
            using (var context = new SoftwareInstallationDatabase())
            {
                Order order = new Order
                {
                    PackageId = model.PackageId,
                    Count = model.Count,
                    Sum = model.Sum,
                    Status = model.Status,
                    DateCreate = model.DateCreate,
                    DateImplement = model.DateImplement,
                };
                context.Orders.Add(order);
                context.SaveChanges();

                CreateModel(model, order);
                context.SaveChanges();
            }
        }

        public void Update(OrderBindingModel model)
        {
            using (var context = new SoftwareInstallationDatabase())
            {
                Order element = context.Orders.FirstOrDefault(rec => rec.Id == model.Id);

                if (element == null)
                {
                    throw new Exception("Элемент не найден");
                }

                element.PackageId = model.PackageId;
                element.Count = model.Count;
                element.Sum = model.Sum;
                element.Status = model.Status;
                element.DateCreate = model.DateCreate;
                element.DateImplement = model.DateImplement;

                CreateModel(model, element);
                context.SaveChanges();
            }
        }
        public void Delete(OrderBindingModel model)
        {
            using (var context = new SoftwareInstallationDatabase())
            {
                Order element = context.Orders.FirstOrDefault(rec => rec.Id == model.Id);

                if (element != null)
                {
                    context.Orders.Remove(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
        }
        private Order CreateModel(OrderBindingModel model, Order order)
        {
            if (model == null)
            {
                return null;
            }

            using (var context = new SoftwareInstallationDatabase())
            {
                Package element = context.Packages.FirstOrDefault(rec => rec.Id == model.PackageId);

                if (element != null)
                {
                    if (element.Orders == null)
                    {
                        element.Orders = new List<Order>();
                    }

                    element.Orders.Add(order);
                    context.Packages.Update(element);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
            return order;
        }
    }
}