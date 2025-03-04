﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Admin.Helper;
using System.Data;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Entities;
using static CommerceProject.Admin.Helper.PageHelper;

namespace CommerceProject.Admin.Controllers
{
    public class KisaLinkController : BaseController
    {
        #region Excel Action
        public class ExcelActionResult : ActionResult
        {
            private readonly DataTable _content;

            public ExcelActionResult(DataTable content)
            {
                _content = content;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ClearContent();
                context.HttpContext.Response.ClearHeaders();
                context.HttpContext.Response.Buffer = true;
                context.HttpContext.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
                context.HttpContext.Response.Write(@"<meta http-equiv=""content-type"" content=""application/vnd.ms-excel; charset=UTF-8"">");
                context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=Rapor.xls");

                //sets font
                context.HttpContext.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
                context.HttpContext.Response.Write("<BR><BR><BR>");
                //sets the table border, cell spacing, border color, font of the text, background, foreground, font height
                context.HttpContext.Response.Write("<Table border='1' bgColor='#ffffff' " +
                  "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
                  "style='font-size:11.0pt; font-family:Calibri; background:white;'> <TR>");

                //am getting my grid's column headers
                int columnscount = _content.Columns.Count;

                for (int j = 0; j < columnscount; j++)
                {   //write in new column
                    context.HttpContext.Response.Write("<Td bgColor='#cacaca'>");
                    //Get column headers  and make it as bold in excel columns
                    context.HttpContext.Response.Write("<B>");
                    context.HttpContext.Response.Write(_content.Columns[j].ColumnName.ToString());
                    context.HttpContext.Response.Write("</B>");
                    context.HttpContext.Response.Write("</Td>");
                }
                context.HttpContext.Response.Write("</TR>");
                foreach (DataRow row in _content.Rows)
                {
                    context.HttpContext.Response.Write("<TR>");

                    for (int i = 0; i < _content.Columns.Count; i++)
                    {
                        context.HttpContext.Response.Write("<Td>");
                        context.HttpContext.Response.Write(row[i].ToString());
                        context.HttpContext.Response.Write("</Td>");
                    }

                    context.HttpContext.Response.Write("</TR>");
                }
                context.HttpContext.Response.Write("</Table>");
                context.HttpContext.Response.Write("</font>");
                context.HttpContext.Response.Flush();
                context.HttpContext.Response.End();
            }
        }
        #endregion

        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        IKisaLinkService KisaLinkService;
        public KisaLinkController(IIcerikAyarService iIcerikAyarService,
                                  IKullaniciService iKullaniciService,
                                  IKisaLinkService iKisaLinkService) : base(iIcerikAyarService,
                                                                            iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            KisaLinkService = iKisaLinkService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = Pages.kisalink_index;
            ViewBag.PageProperties = PageProperties.SetPageProperties("Kısa Link", "Arama", "Kısa Link Arama İşlemleri", "");

            return View();
        }     

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = Pages.kisalink_save;

            if (id == null)
            {
                ViewBag.KisaLink = new KisaLink();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Kısa Link", "Kayıt", "Kısa Link Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.KisaLink = KisaLinkService.FindBy(x => x.KisaLinkId == id).SingleOrDefault();
                ViewBag.PageProperties = PageProperties.SetPageProperties("Kısa Link", "Güncelleme", "Kısa Link Güncelleme İşlemleri", "");
            }
            return View();
        }

        public ActionResult ExcelRaporuAl(string uzunLink, string kisaltilmisLink, int? ziyaretSayisiAlt, int? ziyaretSayisiUst, int aktiflik)
        {
            var sonucListesi = KisaLinkService.FindBy(x =>
           (string.IsNullOrEmpty(uzunLink) || x.UzunLink.Contains(uzunLink)) &&
           (string.IsNullOrEmpty(kisaltilmisLink) || x.KisaltilmisLink.Contains(kisaltilmisLink)) &&
           (
               (ziyaretSayisiAlt == null && ziyaretSayisiUst == null) ||
               (((ziyaretSayisiAlt != null && ziyaretSayisiUst != null) && (x.ZiyaretSayisi >= ziyaretSayisiAlt && x.ZiyaretSayisi <= ziyaretSayisiUst))) ||
               (((ziyaretSayisiAlt != null && ziyaretSayisiUst == null) && (x.ZiyaretSayisi >= ziyaretSayisiAlt))) ||
               (((ziyaretSayisiAlt == null && ziyaretSayisiUst != null) && (x.ZiyaretSayisi <= ziyaretSayisiUst)))
           ) &&
           (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(KisaLink kisaLink)
        {
            try
            {
                KisaLink _kisaLink;

                if (kisaLink.KisaLinkId == 0)
                {
                    _kisaLink = new KisaLink();
                    _kisaLink.UzunLink = kisaLink.UzunLink;
                    _kisaLink.KisaltilmisLink = kisaLink.KisaltilmisLink;
                    _kisaLink.ZiyaretSayisi = 0;
                    _kisaLink.AktifMi = kisaLink.AktifMi;

                    KisaLinkService.Add(_kisaLink);
                }
                else
                {
                    _kisaLink = KisaLinkService.FindBy(x => x.KisaLinkId == kisaLink.KisaLinkId).SingleOrDefault();

                    if (_kisaLink == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _kisaLink.UzunLink = kisaLink.UzunLink;
                    _kisaLink.KisaltilmisLink = kisaLink.KisaltilmisLink;
                    _kisaLink.AktifMi = kisaLink.AktifMi;

                    KisaLinkService.Edit(_kisaLink);
                }

                var flag = KisaLinkService.Save();

                if (flag)
                    return Json(_kisaLink.KisaLinkId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult Sil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _kisaLink = KisaLinkService.FindBy(x => x.KisaLinkId == id).SingleOrDefault();

                if (_kisaLink == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _kisaLink.AktifMi = false;

                KisaLinkService.Edit(_kisaLink);

                var flag = KisaLinkService.Save();

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string uzunLink, string kisaltilmisLink, int? ziyaretSayisiAlt, int? ziyaretSayisiUst, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = KisaLinkService.FindBy(x =>
            (string.IsNullOrEmpty(uzunLink) || x.UzunLink.Contains(uzunLink)) &&
            (string.IsNullOrEmpty(kisaltilmisLink) || x.KisaltilmisLink.Contains(kisaltilmisLink)) &&
            (
                (ziyaretSayisiAlt == null && ziyaretSayisiUst == null) ||
                (((ziyaretSayisiAlt != null && ziyaretSayisiUst != null) && (x.ZiyaretSayisi >= ziyaretSayisiAlt && x.ZiyaretSayisi <= ziyaretSayisiUst))) ||
                (((ziyaretSayisiAlt != null && ziyaretSayisiUst == null) && (x.ZiyaretSayisi >= ziyaretSayisiAlt))) ||
                (((ziyaretSayisiAlt == null && ziyaretSayisiUst != null) && (x.ZiyaretSayisi <= ziyaretSayisiUst)))
            ) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.KisaLinkId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    UzunLink = x.UzunLink,
                    KisaltilmisLink = x.KisaltilmisLink,
                    ZiyaretSayisi = x.ZiyaretSayisi,
                    AktifMi = x.AktifMi,
                    KisaLinkId = x.KisaLinkId,
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LinkOlustur()
        {
            string kisaLink = KisaLinkService.GenerateShortLink();
            return Json(new
            {
                kisaLink = kisaLink,
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}