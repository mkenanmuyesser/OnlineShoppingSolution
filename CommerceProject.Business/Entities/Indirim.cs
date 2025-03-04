//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommerceProject.Business.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Indirim
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Indirim()
        {
            this.IndirimKategori = new HashSet<IndirimKategori>();
            this.IndirimKullanim = new HashSet<IndirimKullanim>();
            this.IndirimUretici = new HashSet<IndirimUretici>();
            this.IndirimUrun = new HashSet<IndirimUrun>();
        }
    
        public int IndirimId { get; set; }
        public int IndirimTipId { get; set; }
        public string Adi { get; set; }
        public decimal IndirimYuzdesi { get; set; }
        public decimal IndirimMiktari { get; set; }
        public Nullable<decimal> MaksimumIndirimMiktarı { get; set; }
        public Nullable<decimal> MinimumIndirimMiktarı { get; set; }
        public Nullable<System.DateTime> BaslangicTarih { get; set; }
        public Nullable<System.DateTime> BitisTarih { get; set; }
        public bool HediyeKartGerektirsinMi { get; set; }
        public string HediyeKartKuponKodu { get; set; }
        public bool AktifMi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IndirimKategori> IndirimKategori { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IndirimKullanim> IndirimKullanim { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IndirimUretici> IndirimUretici { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IndirimUrun> IndirimUrun { get; set; }
    }
}
