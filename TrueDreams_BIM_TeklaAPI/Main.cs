using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Dialog;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Model.Operations;

namespace TrueDreams_BIM_TeklaAPI
{
    public partial class Form1 : ApplicationFormBase
    {
        private readonly Model mymodel; 
      
        public Form1()
        {            
            InitializeComponent();
            base.InitializeForm();            
            mymodel = new Model();
        }

       // 柱脚ベースプレートの自動配置
private void button1_Click(object sender, EventArgs e)
{
    // ログウィンドウ（Form3）を表示
    Form3 f3 = new Form3();
    f3.Show();

    // 処理開始のログを出力
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===柱底プレートの自動作成を開始===");

    // Teklaモデルとの接続を確認
    if (mymodel.GetConnectionStatus())
    {
        // モデル内のすべてのBEAM（部材）オブジェクトを取得
        ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

        // 詳細コンポーネント（ベースプレートなど）を定義するオブジェクト
        Detail d1 = new Detail();

        // 進捗バーの初期化
        progressBar1.Visible = true;
        progressBar1.Maximum = more.GetSize(); // 全部材数を最大値とする
        progressBar1.Value = 0;
        progressBar1.Step = 1;
        int z = 0; // 成功カウント用

        // 各BEAMに対して処理を行う
        foreach (Tekla.Structures.Model.Object obj in more)
        {
            Beam b1 = obj as Beam; // 型キャスト
            if (b1.Name == "COLUMN") // 名前が「COLUMN」の場合のみ処理（柱）
            {
                // 柱のプロファイル（断面情報）を取得
                string pro = b1.Profile.ProfileString;

                // 詳細部材（ベースプレート）コンポーネントの設定
                d1.Name = "ecsk_base_plate2";       // コンポーネント名
                d1.Number = 90001019;               // コンポーネント番号
                d1.LoadAttributesFromFile(pro);     // プロファイルに応じた属性を読み込む

                d1.UpVector = new Vector(0, 0, 1000);           // 上向き方向ベクトル（基準設定）
                d1.DetailType = Tekla.Structures.DetailTypeEnum.END; // 接合位置を「端部」に設定
                d1.SetPrimaryObject(b1);                         // 対象の柱を主オブジェクトに設定
                d1.SetReferencePoint(b1.StartPoint);             // 柱の始点を基準点に設定
                d1.Class = 99;                                   // クラス番号（任意の分類）
                d1.Code = "D19";                                 // コード（属性定義用）

                // モデルにコンポーネントを挿入
                bool result = d1.Insert();
                if (!result)
                {
                    // 挿入失敗時のログ出力
                    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　接線の実行に失敗!");
                }
                else
                {
                    z++; // 成功数カウント
                }
            }

            // 進捗バーを進める
            progressBar1.Value += progressBar1.Step;
        }

        // 処理完了のログを出力
        f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個の接線が実行されました!");
        f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行終了===");

        // 進捗バー非表示にして変更を反映
        progressBar1.Visible = false;
        mymodel.CommitChanges();
    }
    else
    {
        // モデルが開いていない場合の警告
        MessageBox.Show("TEKLAモデルが開いていません");
    }
}


        // 柱と梁の自動接合を行う処理
private void button2_Click(object sender, EventArgs e)
{
    // ログウィンドウを表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===自動柱梁接合の実行を開始します===");

    // モデル内のすべてのBEAMオブジェクトを取得
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

    // 進捗バーの初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;
    int z = 0; // 接合成功数カウント

    // 各オブジェクトに対して処理
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        Beam b1 = obj as Beam;
        if (b1.Name == "COLUMN") // 柱である場合
        {
            // 柱のソリッド形状を取得
            Tekla.Structures.Model.Solid solid1 = b1.GetSolid();

            // 柱の周囲に一定のバッファを取った境界ボックスを作成
            Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(
                solid1.MinimumPoint.X - 100,
                solid1.MinimumPoint.Y - 100,
                solid1.MinimumPoint.Z);

            Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(
                solid1.MaximumPoint.X + 100,
                solid1.MaximumPoint.Y + 100,
                solid1.MaximumPoint.Z);

            Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 = new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);

            // 境界ボックス内に存在するすべてのオブジェクトを取得
            Tekla.Structures.Model.ModelObjectEnumerator objEnum1 = mymodel.GetModelObjectSelector()
                .GetObjectsByBoundingBox(MyAxisAlignedBoundingBox1.MinPoint, MyAxisAlignedBoundingBox1.MaxPoint);

            // 境界内にある梁（BEAM）と接合処理
            foreach (Tekla.Structures.Model.Object oobj in objEnum1)
            {
                if (oobj is Beam)
                {
                    Beam mybeam = oobj as Beam;
                    if (mybeam.Name == "BEAM") // 梁である場合
                    {
                        // プロファイル情報を取得して、ジョイントに読み込む
                        string pro = mybeam.Profile.ProfileString;
                        Tekla.Structures.Model.Connection myPlugin = new Tekla.Structures.Model.Connection();
                        myPlugin.Name = "j90000035";   // コンポーネント名
                        myPlugin.Number = 90000035;   // コンポーネント番号
                        myPlugin.LoadAttributesFromFile(pro); // 属性ファイルを読み込み
                        myPlugin.Class = 99;

                        // 柱を主オブジェクト、梁を従オブジェクトとして設定
                        myPlugin.SetPrimaryObject(b1);
                        myPlugin.SetSecondaryObject(mybeam);

                        // ジョイント（接合コンポーネント）を挿入
                        if (!myPlugin.Insert())
                        {
                            // 失敗時のログ出力
                            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　ジョイントの実行に失敗しました！");
                        }
                        else
                        {
                            z++; // 成功数カウント
                        }
                    }
                }
            }
        }

        // 進捗バーを1ステップ進める
        progressBar1.Value += progressBar1.Step;
    }

    // 結果出力
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個のジョイントを実行しました！");
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行が完了しました===");

    // 進捗バーを非表示にしてモデル変更をコミット
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}


       // 自動内壁作成の実行処理
private void button3_Click(object sender, EventArgs e)
{
    // ログウィンドウを表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===自動内壁作成の実行を開始します===");

    // モデル内のすべてのBEAM（部材）を取得
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
    
    // 入力オブジェクトを一時保存するリスト
    ArrayList myarr = new ArrayList();

    // 進捗バーの初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;
    int z = 0; // 実行成功数カウント

    // 各部材（BEAM）に対して処理
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        myarr.Clear(); // 前のループの残りをクリア
        Beam b1 = obj as Beam;

        // COLUMN（柱）の名前が含まれる部材に限定
        if (b1.Name.Contains("COLUMN") == true)
        {
            // 柱のソリッド情報からバウンディングボックスを定義
            Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
            Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(
                solid1.MinimumPoint.X - 100,
                solid1.MinimumPoint.Y - 100,
                solid1.MinimumPoint.Z);
            Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(
                solid1.MaximumPoint.X + 100,
                solid1.MaximumPoint.Y + 100,
                solid1.MaximumPoint.Z);

            Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 =
                new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);

            // バウンディングボックス内の部材を取得
            Tekla.Structures.Model.ModelObjectEnumerator objEnum1 =
                mymodel.GetModelObjectSelector().GetObjectsByBoundingBox(
                    MyAxisAlignedBoundingBox1.MinPoint, MyAxisAlignedBoundingBox1.MaxPoint);

            // コンポーネント入力の準備
            ComponentInput CI = new ComponentInput();

            // 内壁コンポーネントの作成（標準属性）
            Tekla.Structures.Model.Component myPlugin2 = new Tekla.Structures.Model.Component();
            myPlugin2.Number = 90000097;
            myPlugin2.LoadAttributesFromFile("standard");

            // 主オブジェクトとして柱を追加
            CI.AddInputObject(b1);

            // 周囲の梁（BEAM）を副オブジェクトとして追加
            foreach (Tekla.Structures.Model.Object oobj in objEnum1)
            {
                if (oobj is Beam)
                {
                    Beam mybeam = oobj as Beam;
                    if (mybeam.Name == "BEAM")
                    {
                        myarr.Add(mybeam);
                    }
                }
            }

            // 複数の副オブジェクトをまとめて入力
            CI.AddInputObjects(myarr);

            // コンポーネントに入力を設定し、挿入を試行
            myPlugin2.SetComponentInput(CI);
            z++; // 実行カウント増加

            // 挿入失敗時のログ出力
            if (!myPlugin2.Insert())
                f3.newtext((DateTime.Now.ToShortTimeString()) + "　　ジョイントの実行に失敗しました！");
        }

        // 進捗バーを更新
        progressBar1.Value += progressBar1.Step;
    }

    // 実行結果を出力
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個の処理を実行しました！");
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行が完了しました===");

    // 処理後の後始末
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}



       // Z方向オフセットをゼロにリセットする処理
private void button4_Click(object sender, EventArgs e)
{
    // ログウィンドウを表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===オフセットをゼロにリセット開始===");

    // モデル内のすべてのBEAM（梁）オブジェクトを取得
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

    // 進捗バー初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;

    int z = 0; // リセット成功数

    // 各BEAMに対して処理
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        Beam b1 = obj as Beam;

        // 両端のZ方向オフセットがほぼ同じで、かつゼロでない場合に処理
        if ((Math.Abs(b1.StartPointOffset.Dz - b1.EndPointOffset.Dz) < 1) && b1.StartPointOffset.Dz != 0)
        {
            double off = b1.StartPointOffset.Dz; // 元のオフセット量を記録

            // 元の位置を保つための移動ベクトル（Z方向）
            Vector vector = new Vector(0, 0, off);

            // オフセットをゼロにリセット
            b1.StartPointOffset.Dz = 0;
            b1.EndPointOffset.Dz = 0;

            z++; // 成功数カウント

            b1.Modify(); // モデルを更新
            Operation.MoveObject(b1, vector); // オブジェクトを元の位置に移動
        }

        // 進捗バーを更新
        progressBar1.Value += progressBar1.Step;
    }

    // 結果ログの出力
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個の処理を実行しました！");
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行が完了しました===");

    // モデル更新・後処理
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}



       // 柱のプロファイルに基づく自動色分け処理
private void button5_Click(object sender, EventArgs e)
{
    // ログウィンドウを表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===柱の自動色分けを開始します===");

    // モデル内のすべてのBEAMオブジェクトを取得（2つの列挙体を用意）
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
    ModelObjectEnumerator more2 = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

    // 進捗バーの初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;

    int i = 1; // クラス番号（色）カウント用（1～14）
    bool skip = false; // 既に分類済みかどうかのフラグ
    List<string> slist = new List<string>(); // 登録済みのプロファイル名リスト

    // 各BEAMに対して処理を行う
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        if (i > 14) { i = 1; } // クラス番号が14を超えたら1に戻す
        skip = false;
        Beam b1 = obj as Beam;

        // 柱（COLUMN）であるかを判定
        if (b1.Name.Contains("COLUMN") == true)
        {
            // すでに同じプロファイルを処理済みかチェック
            foreach (string test in slist)
            {
                if (test == b1.Profile.ProfileString)
                {
                    skip = true;
                    break;
                }
                else
                {
                    skip = false;
                }
            }

            // 新しいプロファイルの場合のみ処理
            if (!skip)
            {
                b1.Class = i.ToString(); // クラス（色分け用）を設定
                b1.Modify(); // モデルを更新

                // 同じプロファイルを持つ他の柱にも同じクラスを設定
                foreach (Tekla.Structures.Model.Object obj2 in more2)
                {
                    Beam b2 = obj2 as Beam;
                    if (b2.Name.Contains("COLUMN") == true)
                    {
                        if (b1.Profile.ProfileString == b2.Profile.ProfileString)
                        {
                            b2.Class = i.ToString();
                            b2.Modify();
                        }
                    }
                }

                // 次のクラス番号へ
                i++;
                // このプロファイルを記録して次回スキップできるようにする
                slist.Add(b1.Profile.ProfileString);
            }
        }

        // more2の列挙をリセットして次の検索に備える
        more2.Reset();

        // 進捗バーを更新
        progressBar1.Value += progressBar1.Step;
    }

    // 完了ログを出力
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行が完了しました===");

    // モデル変更を確定し、後処理
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}

       // 梁のプロファイルに基づく自動色分け処理
private void button6_Click(object sender, EventArgs e)
{
    // ログウィンドウを表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===梁の自動色分けを開始します===");

    // モデル内のすべてのBEAMオブジェクトを取得（2重列挙体を使用）
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
    ModelObjectEnumerator more2 = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

    // 進捗バーの初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;

    int i = 1; // Class（色）番号
    bool skip = false;
    List<string> slist = new List<string>(); // 処理済みプロファイルリスト

    // 各BEAMに対して処理
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        if (i > 14) { i = 1; } // クラス番号が14を超えたら1に戻す
        skip = false;
        Beam b1 = obj as Beam;

        // 「BEAM」という名前が含まれる部材（梁）であるか確認
        if (b1.Name.Contains("BEAM") == true)
        {
            // すでに処理済みのプロファイルかチェック
            foreach (string test in slist)
            {
                if (test == b1.Profile.ProfileString)
                {
                    skip = true;
                    break;
                }
                else
                {
                    skip = false;
                }
            }

            // 未分類のプロファイルの場合のみ処理
            if (!skip)
            {
                // Classを設定（色分け）
                b1.Class = i.ToString();
                b1.Modify();

                // 同じプロファイルの梁すべてに同じClassを適用
                foreach (Tekla.Structures.Model.Object obj2 in more2)
                {
                    Beam b2 = obj2 as Beam;
                    if (b2.Name.Contains("BEAM") == true)
                    {
                        if (b1.Profile.ProfileString == b2.Profile.ProfileString)
                        {
                            b2.Class = i.ToString();
                            b2.Modify();
                        }
                    }
                }

                i++; // 次のクラスへ
                slist.Add(b1.Profile.ProfileString); // プロファイルを登録
            }
        }

        more2.Reset(); // 内部ループの列挙子をリセット
        progressBar1.Value += progressBar1.Step; // 進捗更新
    }

    //（未使用の変数：プロファイル文字列出力用）
    string k = "";
    foreach (string a in slist)
    {
        k = k + "\n" + a;
    }

    // 完了メッセージ
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行が完了しました===");
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}


       // 梁同士の自動接合処理
private void button7_Click(object sender, EventArgs e)
{
    // ログウィンドウの表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===梁と梁の自動接合を開始します===");

    // モデル内のすべてのBEAMオブジェクトを2つの列挙体で取得（相互比較のため）
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
    ModelObjectEnumerator more2 = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

    // 進捗バーの初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;

    int i = 0; // more のインデックス
    int z = 0; // 成功した接合数

    // 使用する接合コンポーネントの初期設定
    Tekla.Structures.Model.Connection myPlugin = new Tekla.Structures.Model.Connection
    {
        Name = "j90000032", // 接合コンポーネント名
        Number = 90000032   // コンポーネント番号
    };

    // 各BEAMについて他のBEAMとの接合を試みる
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        Beam b1 = obj as Beam;
        int j = 0; // more2 のインデックス

        if (b1.Name == "BEAM")
        {
            // b1のソリッド形状を取得して、検索用のバウンディングボックスを作成
            Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
            Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(
                solid1.MinimumPoint.X + 100, solid1.MinimumPoint.Y + 100, solid1.MinimumPoint.Z - 100);
            Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(
                solid1.MaximumPoint.X - 100, solid1.MaximumPoint.Y - 100, solid1.MaximumPoint.Z + 100);

            Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 =
                new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);

            Tekla.Structures.Geometry3d.Point spoint;
            Tekla.Structures.Geometry3d.Point epoint;

            // b1に対してすべてのBEAM（b2）を調査
            foreach (Tekla.Structures.Model.Object oobj in more2)
            {
                // 同一ビーム同士の接合を避ける
                if (i == j)
                {
                    j++;
                    continue;
                }

                if (oobj is Beam)
                {
                    Beam mybeam = oobj as Beam;

                    if (mybeam.Name == "BEAM")
                    {
                        // b2の始点・終点を取得
                        spoint = new Tekla.Structures.Geometry3d.Point(
                            mybeam.StartPoint.X, mybeam.StartPoint.Y, mybeam.StartPoint.Z);
                        epoint = new Tekla.Structures.Geometry3d.Point(
                            mybeam.EndPoint.X, mybeam.EndPoint.Y, mybeam.EndPoint.Z);

                        // 始点または終点がb1のバウンディングボックス内にある場合、接合処理を行う
                        if (MyAxisAlignedBoundingBox1.IsInside(spoint) || MyAxisAlignedBoundingBox1.IsInside(epoint))
                        {
                            // プロファイルを読み込んでコンポーネント属性設定
                            string pro = mybeam.Profile.ProfileString;
                            myPlugin.LoadAttributesFromFile(pro);
                            myPlugin.Class = 99;

                            // b1を主オブジェクト、mybeam（b2）を従オブジェクトとして設定
                            myPlugin.SetPrimaryObject(b1);
                            myPlugin.SetSecondaryObject(mybeam);

                            // 接合コンポーネントの挿入
                            if (!myPlugin.Insert())
                            {
                                f3.newtext((DateTime.Now.ToShortTimeString()) + "　　ジョイントを作成できませんでした。");
                            }
                            else
                            {
                                z++; // 成功カウント
                            }
                        }
                    }
                }

                j++;
            }
        }

        progressBar1.Value += progressBar1.Step; // 進捗バー更新
        i++;
        more2.Reset(); // 比較用列挙体のリセット
    }

    // 処理結果のログ表示
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + " 件の接合が完了しました！");
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===プログラムの実行が完了しました===");

    // 後処理
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}




       // 大梁・小梁の重量を集計する処理
private void button8_Click(object sender, EventArgs e)
{
    // ログウィンドウの表示
    Form3 f3 = new Form3();
    f3.Show();
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===大梁・小梁の数量を計算開始==="); // 開始メッセージ

    // モデル内のすべてのBEAMを取得
    ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

    // 進捗バー初期化
    progressBar1.Visible = true;
    progressBar1.Maximum = more.GetSize();
    progressBar1.Value = 0;
    progressBar1.Step = 1;

    int z = 0;                    // カウント用（未使用）
    double sum_b = 0;             // 大梁の合計重量
    double sum_s = 0;             // 小梁の合計重量
    bool skip_count = false;      // 大梁に分類された場合のフラグ
    double weight = 0;            // 単一梁の重量

    // 各BEAMに対して処理
    foreach (Tekla.Structures.Model.Object obj in more)
    {
        Beam b1 = obj as Beam;

        // 対象がBEAMであることを確認
        if (b1.Name == "BEAM")
        {
            // b1のソリッド形状を取得し、その周囲に検索用バウンディングボックスを作成
            Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
            Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(
                solid1.MinimumPoint.X - 100, solid1.MinimumPoint.Y - 100, solid1.MinimumPoint.Z);
            Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(
                solid1.MaximumPoint.X + 100, solid1.MaximumPoint.Y + 100, solid1.MaximumPoint.Z);
            Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 =
                new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);

            // ボックス内に存在するすべてのオブジェクトを取得
            Tekla.Structures.Model.ModelObjectEnumerator objEnum1 =
                mymodel.GetModelObjectSelector().GetObjectsByBoundingBox(
                    MyAxisAlignedBoundingBox1.MinPoint, MyAxisAlignedBoundingBox1.MaxPoint);

            // ボックス内に「COLUMN」が含まれていれば「大梁」としてカウント
            foreach (Tekla.Structures.Model.Object oobj in objEnum1)
            {
                if (oobj is Beam)
                {
                    Beam mybeam = oobj as Beam;
                    if (mybeam.Name == "COLUMN")
                    {
                        b1.GetReportProperty("WEIGHT_NET", ref weight); // 正味重量を取得
                        sum_b += weight;
                        skip_count = true; // 大梁と判定されたためスキップ
                        break;
                    }
                }
            }

            // 小梁と判定された場合
            if (!skip_count)
            {
                b1.GetReportProperty("WEIGHT_NET", ref weight); // 正味重量を取得
                sum_s += weight;
            }
        }

        skip_count = false; // 次のループのためにリセット
        progressBar1.Value += progressBar1.Step;
    }

    // 重量結果をログに出力
    f3.newtext((DateTime.Now.ToShortTimeString()) + "  大梁の合計重量：" + sum_b + "kg");
    f3.newtext((DateTime.Now.ToShortTimeString()) + "  小梁の合計重量：" + sum_s + "kg");
    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===処理が完了しました===");

    // 進捗バー非表示および変更コミット
    progressBar1.Visible = false;
    mymodel.CommitChanges();
}

    }
}
