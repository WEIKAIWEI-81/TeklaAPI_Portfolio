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
        private readonly Model mymodel; //事先宣告載入一個TEKLA模型
      
        public Form1()
        {            
            InitializeComponent();
            base.InitializeForm();            
            mymodel = new Model(); //表單開啟時針對模型做初始化
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行自動建立柱底版===");
            if (mymodel.GetConnectionStatus()) //判斷是否有連動到模型
            {

                ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
                Detail d1 = new Detail(); //細部接頭               

                progressBar1.Visible = true;
                progressBar1.Maximum = more.GetSize();
                progressBar1.Value = 0;
                progressBar1.Step = 1;
                int z = 0;

                foreach (Tekla.Structures.Model.Object obj in more)
                {
                   Beam b1 = obj as Beam;

                        if (b1.Name == "COLUMN")
                        {
                            string pro = b1.Profile.ProfileString;
                            d1.Name = "ecsk_base_plate2"; //指定接頭名稱
                            d1.Number = 90001019; //指定接頭號碼
                            d1.LoadAttributesFromFile(pro);
                            
                            d1.UpVector = new Vector(0, 0, 1000);
                            d1.DetailType = Tekla.Structures.DetailTypeEnum.END;
                            d1.SetPrimaryObject(b1);
                            d1.SetReferencePoint(b1.StartPoint);
                            d1.Class = 99;
                            d1.Code = "D19";
                            bool result = d1.Insert();
                        if (!result) { 
                            
                            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　joint 無法執行!"); 
                        
                         }
                        else
                        {
                            z++;
                        }
                    }

                    progressBar1.Value += progressBar1.Step;

                }
                f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個接頭程式已執行!");
                f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
                progressBar1.Visible = false;
                mymodel.CommitChanges();
            }
            else
            {
                MessageBox.Show("未開啟TEKLA模型");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行自動柱梁接頭===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            int z = 0;

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                Beam b1 = obj as Beam;
                if (b1.Name == "COLUMN")
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
                            if (mybeam.Name == "BEAM")
                            {
                                string pro = mybeam.Profile.ProfileString;
                                Tekla.Structures.Model.Connection myPlugin = new Tekla.Structures.Model.Connection();
                                myPlugin.Name = "j90000035";
                                myPlugin.Number = 90000035;
                                //MessageBox.Show(myBeamArr[i].StartPoint.ToString());
                                myPlugin.LoadAttributesFromFile(pro);
                                myPlugin.Class = 99;
                                myPlugin.SetPrimaryObject(b1);
                                myPlugin.SetSecondaryObject(mybeam);
                                if (!myPlugin.Insert())
                                {
                                    f3.newtext((DateTime.Now.ToShortTimeString()) + "　　joint 無法執行!");
                                }
                                else
                                {
                                    z++;
                                }
                            }
                        }
                        
                    }
                }
                progressBar1.Value += progressBar1.Step;
            }
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個接頭程式已執行!");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            mymodel.CommitChanges();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行自動建立內隔版===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            ArrayList myarr = new ArrayList();

            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            int z = 0;

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                myarr.Clear();
                Beam b1 = obj as Beam;
                if (b1.Name.Contains("COLUMN") == true)
                {
                    Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
                    Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(solid1.MinimumPoint.X - 100, solid1.MinimumPoint.Y - 100, solid1.MinimumPoint.Z);
                    Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(solid1.MaximumPoint.X + 100, solid1.MaximumPoint.Y + 100, solid1.MaximumPoint.Z);
                    Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 = new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);
                    Tekla.Structures.Model.ModelObjectEnumerator objEnum1 = mymodel.GetModelObjectSelector().GetObjectsByBoundingBox(MyAxisAlignedBoundingBox1.MinPoint, MyAxisAlignedBoundingBox1.MaxPoint);

                   
                    ComponentInput CI = new ComponentInput();

                    Tekla.Structures.Model.Component myPlugin2 = new Tekla.Structures.Model.Component();                    
                    myPlugin2.Number = 90000097;
                    myPlugin2.LoadAttributesFromFile("standard");

                    CI.AddInputObject(b1);

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

                   


                    CI.AddInputObjects(myarr);

                    //MessageBox.Show(myBeamArr[i].StartPoint.ToString());                   

                   /* string aa = "";

                    foreach (Beam oobj in myarr)
                    {
                        aa = aa + oobj.Name.ToString() + "/n";
                    }

                    MessageBox.Show(aa);*/



                    myPlugin2.SetComponentInput(CI);
                    z++;
                    if (!myPlugin2.Insert())
                        f3.newtext((DateTime.Now.ToShortTimeString()) + "　　joint 無法執行!");
                }
                progressBar1.Value += progressBar1.Step;
            }
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個程式已執行!");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            mymodel.CommitChanges();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行偏移歸零===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            int z = 0;

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                Beam b1 = obj as Beam;
                if ((Math.Abs(b1.StartPointOffset.Dz - b1.EndPointOffset.Dz) < 1 ) && b1.StartPointOffset.Dz != 0)
                {
                    double off = b1.StartPointOffset.Dz;

                    Vector vector = new Vector(0, 0, off);

                    b1.StartPointOffset.Dz = 0;

                    b1.EndPointOffset.Dz = 0;

                    z++;

                    b1.Modify();
                    Operation.MoveObject(b1, vector);

                    
                }
                progressBar1.Value += progressBar1.Step;
            }
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個動作已執行!");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            mymodel.CommitChanges();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行柱自動顏色分類===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            ModelObjectEnumerator more2 = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);


            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;

            int i = 1;
            bool skip = false;

            List<String> slist = new List<string>();

            foreach (Tekla.Structures.Model.Object obj in more)
            {                
                if (i > 14) { i = 1; }
                skip = false;
                Beam b1 = obj as Beam;
               // MessageBox.Show(skip.ToString());
                if (b1.Name.Contains("COLUMN") == true)
                {
                   // MessageBox.Show(skip.ToString());
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

                    if (skip == true)
                    {

                    }
                    else
                    {
                        b1.Class = i.ToString();
                        b1.Modify();
                        foreach (Tekla.Structures.Model.Object obj2 in more2)
                        {
                            Beam b2 = obj2 as Beam;
                            //MessageBox.Show(b1.Profile.ProfileString + "," + b2.Profile.ProfileString);

                            if (b2.Name.Contains("COLUMN") == true)
                            {
                                if (b1.Profile.ProfileString == b2.Profile.ProfileString)
                                {


                                    b2.Class = i.ToString();

                                    b2.Modify();
                                }
                            }
                        }
                        i++;
                        slist.Add(b1.Profile.ProfileString);
                        
                    }
                }
                more2.Reset();
                progressBar1.Value += progressBar1.Step;
            }



            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            mymodel.CommitChanges();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行梁自動顏色分類===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            ModelObjectEnumerator more2 = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;

            int i = 1;
            bool skip = false;

            List<String> slist = new List<string>();

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                
                if (i > 14) { i = 1; }
                skip = false;
                Beam b1 = obj as Beam;
                // MessageBox.Show(skip.ToString());
                if (b1.Name.Contains("BEAM") == true)
                {
                    // MessageBox.Show(skip.ToString());
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

                    if (skip == true)
                    {

                    }
                    else
                    {
                        b1.Class = i.ToString();
                        b1.Modify();
                        foreach (Tekla.Structures.Model.Object obj2 in more2)
                        {
                            Beam b2 = obj2 as Beam;
                            //MessageBox.Show(b1.Profile.ProfileString + "," + b2.Profile.ProfileString);

                            if (b2.Name.Contains("BEAM") == true)
                            {
                                if (b1.Profile.ProfileString == b2.Profile.ProfileString)
                                {


                                    b2.Class = i.ToString();

                                    b2.Modify();
                                }
                            }
                        }
                        i++;
                        slist.Add(b1.Profile.ProfileString);

                    }
                }
                more2.Reset();
                progressBar1.Value += progressBar1.Step;
            }

            string k = "";
            foreach (string a in slist)
            {
                k = k + "\n" + a;
            }


            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            mymodel.CommitChanges();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始執行自動建立梁梁接頭===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            ModelObjectEnumerator more2 = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            int i = 0;
            int z = 0;

            Tekla.Structures.Model.Connection myPlugin = new Tekla.Structures.Model.Connection
            {
                Name = "j90000032",
                Number = 90000032
            };

            foreach (Tekla.Structures.Model.Object obj in more)
            {                
                Beam b1 = obj as Beam;
                int j = 0;
                if (b1.Name == "BEAM")
                {
                    Tekla.Structures.Model.Solid solid1 = b1.GetSolid();
                    Tekla.Structures.Geometry3d.Point mypoint1 = new Tekla.Structures.Geometry3d.Point(solid1.MinimumPoint.X+100, solid1.MinimumPoint.Y+100, solid1.MinimumPoint.Z-100);
                    Tekla.Structures.Geometry3d.Point mypoint2 = new Tekla.Structures.Geometry3d.Point(solid1.MaximumPoint.X-100, solid1.MaximumPoint.Y-100, solid1.MaximumPoint.Z+100);
                    Tekla.Structures.Geometry3d.AABB MyAxisAlignedBoundingBox1 = new Tekla.Structures.Geometry3d.AABB(mypoint1, mypoint2);
                    Tekla.Structures.Geometry3d.Point spoint;
                    Tekla.Structures.Geometry3d.Point epoint;



                    foreach (Tekla.Structures.Model.Object oobj in more2)
                    {                     

                        if (i == j)
                        {
                            j++;
                            continue;
                        }

                        else if (oobj is Beam)
                        {
                            Beam mybeam = oobj as Beam;


                            if (mybeam.Name == "BEAM")
                            {

                                spoint = new Tekla.Structures.Geometry3d.Point(mybeam.StartPoint.X, mybeam.StartPoint.Y, mybeam.StartPoint.Z);
                                epoint = new Tekla.Structures.Geometry3d.Point(mybeam.EndPoint.X, mybeam.EndPoint.Y, mybeam.EndPoint.Z);


                                if (MyAxisAlignedBoundingBox1.IsInside(spoint) || MyAxisAlignedBoundingBox1.IsInside(epoint))
                                {

                                    string pro = mybeam.Profile.ProfileString;                                                                     
                                    myPlugin.LoadAttributesFromFile(pro);
                                    myPlugin.Class = 99;
                                    myPlugin.SetPrimaryObject(b1);
                                    myPlugin.SetSecondaryObject(mybeam);
                                    if (!myPlugin.Insert())
                                    {
                                        f3.newtext((DateTime.Now.ToShortTimeString()) + "　　joint 無法執行!");
                                    }
                                    else
                                    {
                                        z++;
                                    }

                                }
                            }
                        }
                        j++;
                    }
                }
                progressBar1.Value += progressBar1.Step;
                i++;
                more2.Reset();
            }
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　" + z + "個接頭程式已執行!");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            
            mymodel.CommitChanges();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.Show();
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===開始計算大小梁數量===");
            ModelObjectEnumerator more = mymodel.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            progressBar1.Visible = true;
            progressBar1.Maximum = more.GetSize();
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            int z = 0;
            double sum_b = 0;
            double sum_s = 0;
            bool skip_count = false;
            double weight = 0;

            foreach (Tekla.Structures.Model.Object obj in more)
            {
                Beam b1 = obj as Beam;
                if (b1.Name == "BEAM")
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
                progressBar1.Value += progressBar1.Step;
            }
            f3.newtext((DateTime.Now.ToShortTimeString()) + "  大樑總共" + sum_b + "kg");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "  小樑總共" + sum_s + "kg");
            f3.newtext((DateTime.Now.ToShortTimeString()) + "　　===程式執行完成===");
            progressBar1.Visible = false;
            mymodel.CommitChanges();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();           
        }
    }
}
