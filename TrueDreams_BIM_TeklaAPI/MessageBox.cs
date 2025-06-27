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
    public partial class Form2 : ApplicationFormBase
    {
        private readonly Model mymodel; // Teklaモデルを事前に宣言して読み込み

        public Form2()
        {
            InitializeComponent();
            base.InitializeForm();
            mymodel = new Model(); // フォーム起動時にTeklaモデルを初期化
        }

        // 指定された断面のBEAMの数量と重量を集計
        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　=== " + ColumnsProfileTextBox.Text + " の集計を開始 ===");

            // モデル内のすべてのBEAMを取得
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            int z = 0;
            double sum_b = 0; // 大梁の合計重量
            double sum_s = 0; // 小梁の合計重量
            bool skip_count = false; // 大梁として分類されたかどうかのフラグ
            double weight = 0; // 個別部材の重量

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                Beam b1 = obj as Beam;

                // 指定されたプロファイルと一致するBEAMのみ処理
                if (b1.Name == "BEAM" && b1.Profile.ProfileString == ColumnsProfileTextBox.Text)
                {
                    // b1の周囲に検索ボックスを作成
                    Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
                    Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(
                        solid1.MinimumPoint.X - 100, solid1.MinimumPoint.Y - 100, solid1.MinimumPoint.Z);
                    Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(
                        solid1.MaximumPoint.X + 100, solid1.MaximumPoint.Y + 100, solid1.MaximumPoint.Z);
                    Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 =
                        new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);

                    // ボックス内の要素を取得
                    Tekla.Structures.Model.ModelObjectEnumerator objEnum1 =
                        mymodel.GetModelObjectSelector().GetObjectsByBoundingBox(
                            MyAxisAlignedBoundingBox1.MinPoint, MyAxisAlignedBoundingBox1.MaxPoint);

                    // 周囲に柱（COLUMN）があれば大梁とみなす
                    foreach (Tekla.Structures.Model.Object oobj in objEnum1)
                    {
                        if (oobj is Beam)
                        {
                            Beam mybeam = oobj as Beam;
                            if (mybeam.Name == "COLUMN")
                            {
                                b1.GetReportProperty("WEIGHT_NET", ref weight);
                                sum_b += weight;
                                skip_count = true;
                                break;
                            }
                        }
                    }

                    // 柱がなければ小梁としてカウント
                    if (!skip_count)
                    {
                        b1.GetReportProperty("WEIGHT_NET", ref weight);
                        sum_s += weight;
                    }
                }

                skip_count = false;
            }

            // 結果をログに出力
            f3.newtext((DateTime.Now.ToShortTimeString()) + "  大梁の合計重量：" + sum_b + "kg");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "  小梁の合計重量：" + sum_s + "kg");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===処理が完了しました===");

            mymodel.CommitChanges();
        }

        // プロファイルカタログ選択時、テキストボックスに反映
        private void profileCatalog1_SelectClicked(object sender, EventArgs e)
        {
            profileCatalog1.SelectedProfile = ColumnsProfileTextBox.Text;
        }

        // プロファイル選択が完了したら、テキストボックスに設定
        private void profileCatalog1_SelectionDone(object sender, EventArgs e)
        {
            SetAttributeValue(ColumnsProfileTextBox, profileCatalog1.SelectedProfile);
            ColumnsProfileTextBox.Text = profileCatalog1.SelectedProfile;
        }
    }
}
