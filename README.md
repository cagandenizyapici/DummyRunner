# DummyRunner

Wario Land 4'teki **"The Wario Hop"** mini oyununun C# ve MonoGame ile yeniden yapilmis halidir.
ISÜ - DGD208 Game Programming 2 dersi final projesi olarak gelistirilmistir.

Wario bir lastik ustunde otomatik ilerler; oyuncu sadece ziplayarak onune cikan engelleri tek tek asmaya calisir. Hata yapmadan ne kadar uzun giderse skor o kadar artar.

## Kontroller

| Tus | Islev |
|-----|-------|
| `Space` | Ziplama / Menude oyunu baslatma |
| `R` | Game Over ekraninda tekrar oyna |
| `Esc` | Cikis |

## Ozellikler

- Lastik ustunde sabit konumda, fizik tabanli ziplama
- 7 farkli engel tipi (37. ve 97. engelde hizlanma)
- Her 15 hop'ta madalya
- Score / Medals / Top Score gostergeleri
- Kalici yuksek skor (dosyaya kaydedilir)
- Ses efektleri ve dongusel muzik
- Parallax arka plan ve skor rekoru kusu
- Pixel-art menu ve game over ekranlari

## Proje Yapisi

| Dosya | Sorumlulugu |
|-------|-------------|
| `Game1.cs` | Ana oyun dongusu, durum makinesi, cizim, HUD, sesler |
| `Player.cs` | Wario'nun fizigi, durumlari ve animasyonlari |
| `Animation.cs` | Sprite sheet animasyon yardimci sinifi |
| `Obstacle.cs` | Engel temel sinifi ve turevleri (kalitim) |
| `ObstacleManager.cs` | Engel uretimi, hareketi ve carpisma kontrolu |
| `NumberRenderer.cs` | Skor sayilarinin pixel rakamlarla cizimi |
| `HighScore.cs` | Yuksek skorun dosyaya kaydedilmesi |
| `TextureUtils.cs` | Sprite'larin dolu piksel sinirinin bulunmasi |

## Kullanilan Teknolojiler

- C# / .NET 9
- MonoGame 3.8 (DesktopGL)
