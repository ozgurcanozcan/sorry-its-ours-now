# 01 — Günlük Döngü

Oyunun omurgası. Her aşama **bir karar** sorar; karar sormayan aşama aşama değildir,
menüdür.

| # | Aşama | Sorduğu karar |
|---|-------|---------------|
| 1 | **Ofis** | İş emri seç (adres, borç, tahmini değer, risk, süre) → *hangi riski alıyorsun?* |
| 2 | **Hazırlık** | Kamyon ve alet seç → *neyi feda ediyorsun?* |
| 3 | **Saha** | Evi boşalt → *neyi alıp neyi bırakıyorsun?* |
| 4 | **Depo** | Temizle, tamir et, lot kur → *emeği nereye harcıyorsun?* |
| 5 | **Müzayede** | Rezerv koy, sat → *bugün mü satıyorsun, bekliyor musun?* |
| 6 | **Kapanış** | Hesap, ceza, yatırım → *bugünü mü yarını mı besliyorsun?* |

## Aşamaların durumu

Hangi aşama ne kadar tanımlı:

| Aşama | Durum |
|-------|-------|
| Ofis | İş emri alanları belli, seçim mantığı **açık** |
| Hazırlık | Kavram var, içerik **açık** — hangi kamyonlar, hangi aletler |
| Saha | En tanımlı aşama. Dört kısıt belli (`02-saha.md`) |
| **Depo** | **En az tanımlı aşama.** Temizleme, tamir, lot kurma — üçü de tek satır |
| Müzayede | Mekanikler listeli, derinlik **açık** (`03-muzayede.md`) |
| Kapanış | Borç sistemi tanımlı (`04-ekonomi.md`), gerisi açık |

## Bir günün süresi

**Açık.** Bir oyun günü gerçek zamanda ne kadar sürmeli?

Bilinen kısıt: saha ziyareti için 8 dakika konuşuldu. Depo ve müzayede eklenince
bir gün 12–15 dakikaya çıkar. Oyuncunun bir oturuşta kaç gün oynayacağı ve
kampanyanın 15–20 görevinin kaç saat ettiği buna bağlı.

## Açık sorular

- [ ] **Depo aşaması gerçekten var mı?** Saha ile müzayede arasında ayrı bir
      aşama mı, yoksa müzayede ekranının bir sekmesi mi? Ayrı aşamaysa oyuncu
      orada ne yapıyor, ne kadar süre geçiriyor?
- [ ] Bir günde kaç iş emri alınabiliyor? Tek mi, birden fazla mı?
- [ ] Oyuncu bir işi reddedebiliyor mu? Reddetmenin bedeli ne?
- [ ] Aşamalar arası geçiş serbest mi (istediğinde müzayedeye git), yoksa sıralı mı?
- [ ] Bir oyun günü gerçek zamanda kaç dakika?
