﻿using Com.DanLiris.Service.Purchasing.Lib.Models.Expedition;
using Com.DanLiris.Service.Purchasing.Lib.Utilities;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.Expedition;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.PDFTemplates
{
    public class PPHBankExpenditureNotePDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(PPHBankExpenditureNote model, int clientTimeZoneOffset)
        {
            const int MARGIN = 20;

            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);

            Document document = new Document(PageSize.A4, MARGIN, MARGIN, MARGIN, MARGIN);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            #region Header

            PdfPTable headerTable = new PdfPTable(3);
            PdfPCell cellHeader = new PdfPCell() { Border = Rectangle.NO_BORDER };
            PdfPCell cellHeaderCS3 = new PdfPCell() { Border = Rectangle.NO_BORDER, Colspan = 3 };
            float[] widths = new float[] { 10f, 10f, 10f };
            headerTable.SetWidths(widths);
            headerTable.WidthPercentage = 100;

            cellHeaderCS3.Phrase = new Phrase("DANLIRIS", normal_font);
            headerTable.AddCell(cellHeaderCS3);

            cellHeader.Phrase = new Phrase("INDUSTRIAL & TRADING CO.LTD.", normal_font);
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("BUKTI PENGELUARAN BANK", bold_font);
            cellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("", normal_font);
            headerTable.AddCell(cellHeader);

            cellHeaderCS3.Phrase = new Phrase("Kel. Banaran (Selatan Laweyan)", normal_font);
            headerTable.AddCell(cellHeaderCS3);

            cellHeader.Phrase = new Phrase("Telp. 714400, 719113", normal_font);
            cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("", normal_font);
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("No Dokumen : " + model.No, normal_font);
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("SOLO - INDONESIA 57100", normal_font);
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("", normal_font);
            headerTable.AddCell(cellHeader);

            cellHeader.Phrase = new Phrase("Pasal PPH     : " + model.IncomeTaxName, normal_font);
            headerTable.AddCell(cellHeader);

            document.Add(headerTable);
            document.Add(new Paragraph("\n"));

            #endregion Header

            #region Body

            PdfPTable bodyTable = new PdfPTable(7);
            PdfPCell bodyCell = new PdfPCell();

            float[] widthsBody = new float[] { 5f, 12f, 10f, 5f, 5f, 15f, 15f };
            bodyTable.SetWidths(widthsBody);
            bodyTable.WidthPercentage = 100;

            bodyCell.Colspan = 7;
            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            bodyCell.Phrase = new Phrase("Bank : " + model.BankName + " " +  model.BankAccountName + " " + model.BankAccountNumber + " " + model.Currency, normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
            bodyCell.Colspan = 1;
            bodyCell.Rowspan = 2;
            bodyCell.Phrase = new Phrase("No.", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 4;
            bodyCell.Rowspan = 1;
            bodyCell.Phrase = new Phrase("Uraian", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.Rowspan = 2;
            bodyCell.Phrase = new Phrase("PPH (" + model.IncomeTaxRate  + "%)", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("DPP", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.Rowspan = 1;
            bodyCell.Phrase = new Phrase("No. SPB", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("Supplier", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("Unit", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Phrase = new Phrase("Mata Uang", normal_font);
            bodyTable.AddCell(bodyCell);

            int index = 1;
            double totalDPP = 0, totalPPH = 0;

            Dictionary<string, double> units = new Dictionary<string, double>();

            model.Items = model.Items.OrderBy(p => p.PurchasingDocumentExpedition.SupplierName).ToList();

            foreach (PPHBankExpenditureNoteItem item in model.Items)
            {
                var pdeItems = item.PurchasingDocumentExpedition.Items
                    .GroupBy(m => new { m.UnitCode, m.UnitName })
                    .Select(s => new
                    {
                        UnitCode = s.First().UnitCode,
                        UnitName = s.First().UnitName,
                        TotalDPP = s.Sum(d => d.Price),
                        TotalPPH = (s.Sum(d => d.Price) * model.IncomeTaxRate) / 100
                    });

                foreach (var pdeItem in pdeItems)
                {
                    bodyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    bodyCell.VerticalAlignment = Element.ALIGN_TOP;
                    bodyCell.Phrase = new Phrase((index++).ToString(), normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    bodyCell.Phrase = new Phrase(item.PurchasingDocumentExpedition.UnitPaymentOrderNo, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(item.PurchasingDocumentExpedition.SupplierName, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(pdeItem.UnitCode, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(item.PurchasingDocumentExpedition.Currency, normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    bodyCell.Phrase = new Phrase(string.Format("{0:n4}", pdeItem.TotalPPH), normal_font);
                    bodyTable.AddCell(bodyCell);

                    bodyCell.Phrase = new Phrase(string.Format("{0:n4}", pdeItem.TotalDPP), normal_font);
                    bodyTable.AddCell(bodyCell);

                    if (units.ContainsKey(pdeItem.UnitCode))
                    {
                        units[pdeItem.UnitCode] += pdeItem.TotalPPH;
                    }
                    else
                    {
                        units.Add(pdeItem.UnitCode, pdeItem.TotalPPH);
                    }

                    totalPPH += pdeItem.TotalPPH;
                    totalDPP += pdeItem.TotalDPP;
                }
            }

            bodyCell.Colspan = 4;
            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyCell.Phrase = new Phrase("Total", normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.Colspan = 1;
            bodyCell.HorizontalAlignment = Element.ALIGN_LEFT;
            bodyCell.Phrase = new Phrase(model.Currency, normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", totalPPH), normal_font);
            bodyTable.AddCell(bodyCell);

            bodyCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            bodyCell.Phrase = new Phrase(string.Format("{0:n4}", totalDPP), normal_font);
            bodyTable.AddCell(bodyCell);

            document.Add(bodyTable);
            document.Add(new Paragraph("\n"));

            #endregion Body

            #region BodyFooter

            PdfPTable bodyFooterTable = new PdfPTable(6);
            PdfPCell bodyFooterCell = new PdfPCell() { Border = Rectangle.NO_BORDER };
            bodyFooterTable.WidthPercentage = 100;

            bodyFooterCell.Colspan = 6;
            bodyFooterCell.Phrase = new Phrase("Rincian PPH per bagian:", normal_font);
            bodyFooterTable.AddCell(bodyFooterCell);

            bodyFooterCell.Colspan = 1;


            foreach (var unit in units)
            {
                bodyFooterCell.Colspan = 1;

                bodyFooterCell.Phrase = new Phrase(unit.Key, normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Phrase = new Phrase("= " + string.Format("{0:n4}", unit.Value), normal_font);
                bodyFooterTable.AddCell(bodyFooterCell);

                bodyFooterCell.Colspan = 4;
                bodyFooterCell.Phrase = new Phrase("");
                bodyFooterTable.AddCell(bodyFooterCell);
            }

            document.Add(bodyFooterTable);
            document.Add(new Paragraph("\n"));

            document.Add(new Phrase(model.Currency + " " + NumberToTextIDN.terbilang(totalPPH), normal_font));
            document.Add(new Paragraph("\n"));

            #endregion BodyFooter

            #region Footer

            PdfPTable footerTable = new PdfPTable(2);
            PdfPCell cellFooter = new PdfPCell() { Border = Rectangle.NO_BORDER };

            float[] widthsFooter = new float[] { 10f, 10f };
            footerTable.SetWidths(widthsFooter);
            footerTable.WidthPercentage = 100;

            cellFooter.Phrase = new Phrase("Dikeluarkan dengan cek/BG No. : " + model.BGNo, normal_font);
            footerTable.AddCell(cellFooter);

            PdfPTable receiverTable = new PdfPTable(1);
            PdfPCell cellReceiver = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellReceiver.Phrase = new Phrase("Sukoharjo, " + DateTimeOffset.UtcNow.AddHours(clientTimeZoneOffset).ToString("dd MMM yyyy"), normal_font);
            receiverTable.AddCell(cellReceiver);

            cellReceiver.FixedHeight = 60;
            cellReceiver.Phrase = new Phrase("Penerima", normal_font);
            receiverTable.AddCell(cellReceiver);

            cellReceiver.FixedHeight = 20;
            cellReceiver.Phrase = new Phrase("(__________________________)");
            receiverTable.AddCell(cellReceiver);

            cellReceiver.Phrase = new Phrase("Nama terang", normal_font);
            receiverTable.AddCell(cellReceiver);

            footerTable.AddCell(new PdfPCell(receiverTable) { Rowspan = 3, Border = Rectangle.NO_BORDER });

            PdfPTable signatureTable = new PdfPTable(3);
            PdfPCell signatureCell = new PdfPCell() { HorizontalAlignment = Element.ALIGN_CENTER };
            signatureCell.Phrase = new Phrase("Bag. Keuangan", normal_font);
            signatureTable.AddCell(signatureCell);

            signatureCell.Phrase = new Phrase("Bag. Akuntansi", normal_font);
            signatureTable.AddCell(signatureCell);

            signatureCell.Phrase = new Phrase("Direksi", normal_font);
            signatureTable.AddCell(signatureCell);

            signatureTable.AddCell(new PdfPCell() { FixedHeight = 40 });
            signatureTable.AddCell(new PdfPCell() { FixedHeight = 40 });
            signatureTable.AddCell(new PdfPCell() { FixedHeight = 40 });

            footerTable.AddCell(new PdfPCell(signatureTable));

            cellFooter.Phrase = new Phrase("", normal_font);
            footerTable.AddCell(cellFooter);
            document.Add(footerTable);

            #endregion Footer

            document.Close();

            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}