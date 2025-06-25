# Tekla Open API by KAIWEI

## 💡 プロジェクト概要
Tekla StructuresのOpen APIを用いた、開発した自動化ツールです。
アイコン上のボタンを押すだけで、Teklaモデル上の各種配置操作を自動化することができます。

### 実装機能:
1. 柱足ベースプレートの自動配置
2. 柱-梁のジョイント自動配置
3. 梁-梁のジョイント自動配置
4. 内部隔壁の自動配置
5. 梁の重量計算 (大梁と小梁の分別)
6. 柱、梁の自動色分け表示
7. オフセット重計済のリセット

---

## 🔧 技術スタック
- C# (.NET Framework)
- Tekla Structures Open API
- Windows Forms

---

## 🖼️ UIイメージ
### 📌 メインメニュー
![image](https://github.com/user-attachments/assets/576895e7-bbee-45fe-92c2-7f15783ff968)

### 📌 プログラム結果レポート
![image](https://github.com/user-attachments/assets/370eb587-eb1b-4af1-83c6-f6b33b35ee6d)

---

## 📁 ファイル構成
```
TrueDreams_BIM_TeklaAPI
├── Main.cs         # メインアプリ / UIロジック
└── TeklaMeassageBox.cs         # ログ出力用ウィンドウ
```

---

## 🔍 ユースケース
現場では数千個の接合部を手動で配置していましたが、
本ツールによって数分で完了する自動化を実現しました。

- テクラ上で開いたプロジェクトに対し、柱/梁/隔壁/ジョイント/色分けを統一管理
- UIは簡単なボタン操作で構成
- 実行状況をログとして表示
- 出力はTeklaMeassageBox.csで矩形表示

---

## 📍 開発者
**Developed by KAIWEI**  
BIMエンジニア / デジタル化ツール開発者  

---

## 📄 ライセンス
MIT License
