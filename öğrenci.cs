using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OgrenciDersYonetimSistemi
{
    // IPerson Interface
    interface IPerson
    {
        int Id { get; set; }
        string AdSoyad { get; set; }
        void BilgiGoster();
    }

    // Temel Sınıf (Base Class)
    abstract class Kisi : IPerson
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }

        protected Kisi(int id, string adSoyad)
        {
            Id = id;
            AdSoyad = adSoyad;
        }

        public abstract void BilgiGoster();
    }

    // Öğrenci Sınıfı
    class Ogrenci : Kisi
    {
        public Ogrenci(int id, string adSoyad) : base(id, adSoyad) { }

        public override void BilgiGoster()
        {
            Console.WriteLine($"Öğrenci ID: {Id}, Ad Soyad: {AdSoyad}");
        }
    }

    // Öğretim Görevlisi Sınıfı
    class OgretimGorevlisi : Kisi
    {
        public string Unvan { get; set; }

        public OgretimGorevlisi(int id, string adSoyad, string unvan) : base(id, adSoyad)
        {
            Unvan = unvan;
        }

        public override void BilgiGoster()
        {
            Console.WriteLine($"Öğretim Görevlisi ID: {Id}, Ad Soyad: {AdSoyad}, Ünvan: {Unvan}");
        }
    }

    // Ders Sınıfı
    class Ders
    {
        public string Ad { get; set; }
        public int Kredi { get; set; }
        public OgretimGorevlisi OgretimGorevlisi { get; set; }
        public List<Ogrenci> Ogrenciler { get; private set; }

        public Ders(string ad, int kredi, OgretimGorevlisi ogretimGorevlisi)
        {
            Ad = ad;
            Kredi = kredi;
            OgretimGorevlisi = ogretimGorevlisi;
            Ogrenciler = new List<Ogrenci>();
        }

        public void OgrenciEkle(Ogrenci ogrenci)
        {
            Ogrenciler.Add(ogrenci);
        }

        public void BilgiGoster()
        {
            Console.WriteLine($"Ders Adı: {Ad}, Kredi: {Kredi}");
            Console.WriteLine($"Dersi Veren: {OgretimGorevlisi.AdSoyad} ({OgretimGorevlisi.Unvan})");
            Console.WriteLine("Kayıtlı Öğrenciler:");
            foreach (var ogrenci in Ogrenciler)
            {
                ogrenci.BilgiGoster();
            }
        }
    }

    // Dosya İşlemleri Yardımcı Sınıfı
    static class DosyaIslemleri
    {
        public static void JsonKaydet<T>(string dosyaAdi, List<T> veriler)
        {
            var json = JsonSerializer.Serialize(veriler, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dosyaAdi, json);
        }

        public static List<T> JsonOku<T>(string dosyaAdi)
        {
            if (!File.Exists(dosyaAdi))
                return new List<T>();

            var json = File.ReadAllText(dosyaAdi);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
    }

    // Ana Program
    class Program
    {
        static List<Ogrenci> ogrenciler = DosyaIslemleri.JsonOku<Ogrenci>("ogrenciler.json");
        static List<OgretimGorevlisi> ogretimGorevlileri = DosyaIslemleri.JsonOku<OgretimGorevlisi>("ogretimGorevlileri.json");
        static List<Ders> dersler = new List<Ders>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Öğrenci ve Ders Yönetim Sistemi ---");
                Console.WriteLine("1. Yeni Öğrenci Ekle");
                Console.WriteLine("2. Yeni Öğretim Görevlisi Ekle");
                Console.WriteLine("3. Yeni Ders Ekle");
                Console.WriteLine("4. Derslere Öğrenci Ekle");
                Console.WriteLine("5. Ders Bilgilerini Listele");
                Console.WriteLine("6. Verileri Kaydet");
                Console.WriteLine("0. Çıkış");
                Console.Write("Seçiminiz: ");
                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        OgrenciEkle();
                        break;
                    case "2":
                        OgretimGorevlisiEkle();
                        break;
                    case "3":
                        DersEkle();
                        break;
                    case "4":
                        DerseOgrenciEkle();
                        break;
                    case "5":
                        DersleriListele();
                        break;
                    case "6":
                        VerileriKaydet();
                        break;
                    case "0":
                        Console.WriteLine("Sistemden çıkılıyor...");
                        VerileriKaydet();
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim! Tekrar deneyin.");
                        break;
                }
            }
        }

        static void OgrenciEkle()
        {
            Console.Write("Öğrenci ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Öğrenci Ad Soyad: ");
            string adSoyad = Console.ReadLine();
            ogrenciler.Add(new Ogrenci(id, adSoyad));
            Console.WriteLine("Öğrenci başarıyla eklendi.");
        }

        static void OgretimGorevlisiEkle()
        {
            Console.Write("Öğretim Görevlisi ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Öğretim Görevlisi Ad Soyad: ");
            string adSoyad = Console.ReadLine();
            Console.Write("Ünvan: ");
            string unvan = Console.ReadLine();
            ogretimGorevlileri.Add(new OgretimGorevlisi(id, adSoyad, unvan));
            Console.WriteLine("Öğretim Görevlisi başarıyla eklendi.");
        }

        static void DersEkle()
        {
            Console.Write("Ders Adı: ");
            string ad = Console.ReadLine();
            Console.Write("Ders Kredisi: ");
            int kredi = int.Parse(Console.ReadLine());

            Console.WriteLine("Mevcut Öğretim Görevlileri:");
            for (int i = 0; i < ogretimGorevlileri.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {ogretimGorevlileri[i].AdSoyad}");
            }
            Console.Write("Dersi veren öğretim görevlisinin numarası: ");
            int secim = int.Parse(Console.ReadLine()) - 1;

            if (secim >= 0 && secim < ogretimGorevlileri.Count)
            {
                dersler.Add(new Ders(ad, kredi, ogretimGorevlileri[secim]));
                Console.WriteLine("Ders başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("Geçersiz seçim!");
            }
        }

        static void DerseOgrenciEkle()
        {
            Console.WriteLine("Mevcut Dersler:");
            for (int i = 0; i < dersler.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {dersler[i].Ad}");
            }
            Console.Write("Öğrenci eklemek istediğiniz dersin numarası: ");
            int dersSecim = int.Parse(Console.ReadLine()) - 1;

            if (dersSecim >= 0 && dersSecim < dersler.Count)
            {
                Console.WriteLine("Mevcut Öğrenciler:");
                for (int i = 0; i < ogrenciler.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {ogrenciler[i].AdSoyad}");
                }
                Console.Write("Derse eklemek istediğiniz öğrencinin numarası: ");
                int ogrenciSecim = int.Parse(Console.ReadLine()) - 1;

                if (ogrenciSecim >= 0 && ogrenciSecim < ogrenciler.Count)
                {
                    dersler[dersSecim].OgrenciEkle(ogrenciler[ogrenciSecim]);
                    Console.WriteLine("Öğrenci derse başarıyla eklendi.");
                }
                else
                {
                    Console.WriteLine("Geçersiz seçim!");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz seçim!");
            }
        }

        static void DersleriListele()
        {
            foreach (var ders in dersler)
            {
                ders.BilgiGoster();
                Console.WriteLine();
            }
        }

        static void VerileriKaydet()
        {
            DosyaIslemleri.JsonKaydet("ogrenciler.json", ogrenciler);
            DosyaIslemleri.JsonKaydet("ogretimGorevlileri.json", ogretimGorevlileri);
            Console.WriteLine("Veriler kaydedildi.");
        }
    }
}

