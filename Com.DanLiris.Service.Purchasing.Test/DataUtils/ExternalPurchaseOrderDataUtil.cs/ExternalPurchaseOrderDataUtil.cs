﻿using Com.DanLiris.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.DanLiris.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.DanLiris.Service.Purchasing.Lib.Models.InternalPurchaseOrderModel;
using Com.DanLiris.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtil.cs
{
    public class ExternalPurchaseOrderDataUtil
    {
        private InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil;
        private readonly ExternalPurchaseOrderFacade facade;

        public ExternalPurchaseOrderDataUtil( ExternalPurchaseOrderFacade facade, InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil)
        {
            this.facade = facade;
            this.internalPurchaseOrderDataUtil = internalPurchaseOrderDataUtil;
        }

        public async Task<ExternalPurchaseOrder> GetNewData(string user)
        {
            InternalPurchaseOrder internalPurchaseOrder = await internalPurchaseOrderDataUtil.GetTestData(user);
            List<ExternalPurchaseOrderDetail> detail = new List<ExternalPurchaseOrderDetail>();
            foreach (var POdetail in internalPurchaseOrder.Items)
            {
                detail.Add(new ExternalPurchaseOrderDetail
                {
                    POItemId = POdetail.Id,
                    PRItemId = Convert.ToInt64(POdetail.PRItemId),
                    ProductId = "ProductId",
                    ProductCode = "ProductCode",
                    ProductName = "ProductName",
                    DefaultQuantity = 10,
                    DealUomId = "UomId",
                    DealUomUnit = "Uom",
                    ProductRemark = "Remark",
                    PriceBeforeTax = 1000,
                    PricePerDealUnit = 200,
                    DealQuantity = POdetail.Quantity
                });
            }
            List<ExternalPurchaseOrderItem> items = new List<ExternalPurchaseOrderItem>();
            items.Add(new ExternalPurchaseOrderItem
            {
                POId = internalPurchaseOrder.Id,
                PRId = Convert.ToInt64(internalPurchaseOrder.PRId),
                PONo = internalPurchaseOrder.PONo,
                PRNo = internalPurchaseOrder.PRNo,
                UnitCode = "unitcode",
                UnitName = "unit",
                UnitId = "unitId",
                Details = detail
            });
            
            return new ExternalPurchaseOrder
            {
                CurrencyCode = "CurrencyCode",
                CurrencyId = "CurrencyId",
                CurrencyRate = "CurrencyRate",
                UnitId = "UnitId",
                UnitCode = "UnitCode",
                UnitName = "UnitName",
                DivisionId = "DivisionId",
                DivisionCode = "DivisionCode",
                DivisionName = "DivisionName",
                FreightCostBy ="test",
                DeliveryDate = DateTime.Now,
                OrderDate = DateTime.Now,
                SupplierCode ="sup",
                SupplierId ="supId",
                SupplierName ="Supplier",
                PaymentMethod = "test",
                Remark = "Remark",
                Items = items
            };
        }

        public async Task<ExternalPurchaseOrder> GetTestData(string user)
        {
            ExternalPurchaseOrder externalPurchaseOrder = await GetNewData(user);

            await facade.Create(externalPurchaseOrder, user);

            return externalPurchaseOrder;
        }


    }
}
