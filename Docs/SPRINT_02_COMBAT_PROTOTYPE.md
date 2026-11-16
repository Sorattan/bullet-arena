# Bullet Arena — İkinci ilerleme için savaş prototipi

**Unity 6.3 LTS / URP • feature/combat-wave-prototype**

## Eklenen altyapı
- PlayerShooter ve BulletProjectile: sol tıklama, mermi ve isabet.
- EnemyAgent: açık test arenasında takip, yakın mesafe saldırısı, hasar.
- PlayerHealth: can, hasar, ölüm.
- WaveManager: üç örnek dalga (3, 4, 5 düşman), dalgalar arası geçiş, basit HUD.
- WaveTelemetry: süre, alınan hasar, düşman sayısı ve başarı durumunu CSV olarak kaydetme.
- PrototypeSceneBuilder: Arena sahnesine prefab ve bileşenleri tek işlemle ekleyen Unity editör aracı.

## Unity'de uygulama
1. `git fetch origin` ve `git switch --track origin/feature/combat-wave-prototype` (yeni dal için).
2. `Assets/_Project/Scenes/Arena.unity` sahnesini açın.
3. Unity menüsü: **Tools → Bullet Arena → Prepare Combat Prototype**.
4. Play: WASD + fareyle yönelme, sol tık ile ateş. Kırmızı düşmanlar yaklaşır.
5. Dalga tamamlanınca Console'da CSV konumu görünür: `Application.persistentDataPath/bullet_arena_waves.csv`.
6. Unity'nin oluşturduğu prefab/materyal/sahne güncellemelerini ve .meta dosyalarını commit'leyin.

## Doğrulama listesi
- [ ] Hatasız derleme ve sahne hazırlama
- [ ] İzometrik kamera takibi ve hareket
- [ ] Atış, düşmana hasar, düşmanın yok edilmesi
- [ ] Düşmanın yaklaşması ve oyuncunun can kaybetmesi
- [ ] Üç dalganın sıralı çalışması
- [ ] Başarılı/başarısız dalga CSV satırı

## Kapsam / sınırlılıklar
Bu paket **kod geliştirmesidir**; Unity'de yerel Play Mode testleri tamamlanmadan
"çalıştığı doğrulandı" denilemez. Düşmanlar henüz NavMesh kullanmaz.
Farklı düşmanlar, upgrade, boss, DDA ve katılımcı deneyi tamamlanmamıştır.
TÜBİTAK 2209-A kapsamında ayrı bir **beş dalgalık deney modu** hazırlanacaktır.
Sabit ve DDA koşulları aynı düşman toplamını kullanacak, yalnızca düşman doğma
hızı oyuncu performansına göre dalga sonunda değişecektir. Buradaki üç dalga,
akademik deney değil, teknik prototip içindir.
