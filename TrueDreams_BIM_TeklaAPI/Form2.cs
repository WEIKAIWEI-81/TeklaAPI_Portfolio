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
        private readonly Model mymodel; //事先宣告載入一個TEKLA模型
        public Form2()
        {
            InitializeComponent();
            base.InitializeForm();
            mymodel = new Model(); //表單開啟時針對模型做初始化
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始計算"+ ColumnsProfileTextBox.Text +"數量 ===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
                       
            int z = 0;
            double sum_b = 0;
            double sum_s = 0;
            bool skip_count = false;
            double weight = 0;

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                Beam b1 = obj as Beam;
                if (b1.Name == "BEAM" && b1.Profile.ProfileString== ColumnsProfileTextBox.Text)
                {
                    Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
                    Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(solid1.MinimumPoint.X - 100, solid1.MinimumPoint.Y - 100, solid1.MinimumPoint.Z);
                    Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(solid1.MaximumPoint.X + 100, solid1.MaximumPoint.Y + 100, solid1.MaximumPoint.Z);
                    Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 = new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);
                    Tekla.Structures.Model.ModelObjectEnumerator objEnum1 = mymodel.GetModelObjectSelector().GetObjectsByBoundingBox(MyAxisAlignedBoundingBox1.MinPoint, MyAxisAlignedBoundingBox1.MaxPoint);

                    foreach (Tekla.Structures.Model.Object oobj in objEnum1)
                    {
                        if (oobj is Beam)
                        {
                            Beam mybeam = oobj as Beam;
                            if (mybeam.Name == "COLUMN")
                            {
                                b1.GetReportProperty("WEIGHT_NET", ref weight);
                                sum_b = sum_b + weight;
                                skip_count = true;
                                break;
                            }
                        }

                    }

                    if (skip_count == false)
                    {
                        b1.GetReportProperty("WEIGHT_NET", ref weight);
                        sum_s = sum_s + weight;
                    }


                }


                skip_count = false;              
            }
            f3.newtext((DateTime.Now.ToShortTimeString()) + "  大樑總共" + sum_b + "kg");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "  小樑總共" + sum_s + "kg");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
           
            mymodel.CommitChanges();
        }               

        private void profileCatalog1_SelectClicked(object sender, EventArgs e)
        {
            profileCatalog1.SelectedProfile = ColumnsProfileTextBox.Text;
        }

        private void profileCatalog1_SelectionDone(object sender, EventArgs e)
        {
            SetAttributeValue(ColumnsProfileTextBox, profileCatalog1.SelectedProfile);
            ColumnsProfileTextBox.Text = profileCatalog1.SelectedProfile;
        }
    }
}
