using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using study.Class;

namespace study
{
    public partial class Form1 : Form
    {

        #region 전역변수

        List<Dictionary<String, object>> listItem = new List<Dictionary<string, object>>();

        public string[] listview_columnTitle = { "인식여부", "파일명", "Path", "인식값", "좌표값" };

        public static string[] _1D_Name = { "CODE39", "CODE93", "CODE128", "CODABAR", "EAN", "UPC" };
        public static string[] _2D_Name = { "DATAMATRIX", "QRCODE" };
        private int[] _1D_Barcode = { 0x0001, 0x0002, 0x0004, 0x0008, 0x0020, 0x0040 };
        private int[] _2D_Barcode = { 0x0200, 0x0400 };

        #endregion

        #region listview 옵션
        private void ListView_option()
        {
            listView1.View = View.Details;
            listView1.Columns.Add("", 20, HorizontalAlignment.Center);    //0
            listView1.Columns.Add(listview_columnTitle[0], 60, HorizontalAlignment.Center); // 1 인식여부
            listView1.Columns.Add(listview_columnTitle[1], 250, HorizontalAlignment.Center);    //2 파일명
            //listView1.Columns.Add(listview_columnTitle[2], 150, HorizontalAlignment.Center);    //3 파일경로
            listView1.Columns.Add(listview_columnTitle[2]).Width = 0; ;    //3
            listView1.Columns.Add(listview_columnTitle[3], 250, HorizontalAlignment.Center);   //4 인식값
            //listView1.Columns.Add(listview_columnTitle[4], 150, HorizontalAlignment.Center);    //5 좌표
            listView1.Columns.Add(listview_columnTitle[4]).Width=0; // 5

            listView1.Items.Clear();
        }
        #endregion

        #region 폼 로드
        private void Form1_Load(object sender, EventArgs e)
        {
            ListView_option();
            Set_RealEyeView2();
            Set_RealEyeView();
        }

        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region 불러오기
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                listItem.Clear();
            }

            // 바코드 타입 구분 변수
            int type = 0; // 한개 체크할 경우 사용

            // 함수 반환 값 넣을 배열
            String[] result = null;

            //DII 모듈이 있는 경로를 지정한다.
            //Directory.SetCurrentDirectory(@"C:\Users\hakju\Desktop\바코드모듈\03.engine");
            string enginePath = Application.StartupPath + @"\" + "engine";
            Directory.SetCurrentDirectory(enginePath);

            // OpenFileDialog객체 생성 및 filter 정의
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "모든파일|*.*|JPG|*.jpg|PNG|*.png" })
            {
                // 파일 복수 선택 가능
                ofd.Multiselect = true;

                // 파일 선택하면
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 파일 개수만큼 roop
                    for (int i = 0; i < ofd.FileNames.Length; i++)
                    {
                        try
                        {
                            // 바코드 decoding
                            if (type != 0)
                            {
                                result = F_BarcodeRead(ofd.FileNames[i], type);
                            }
                            else
                            {
                                result = null;

                                for (int j = 0; j < _1D_Barcode.Length; j++)
                                {
                                    if (result == null)
                                    {
                                        result = F_BarcodeRead(ofd.FileNames[i], _1D_Barcode[j]);
                                    }
                                }
                                if (result == null)
                                {
                                    for (int j = 0; j < _2D_Barcode.Length; j++)
                                    {
                                        if (result == null)
                                        {
                                            result = F_BarcodeRead(ofd.FileNames[i], _2D_Barcode[j]);
                                        }
                                    }
                                }
                            }
                            // listView 업데이트 시작
                            listView1.BeginUpdate();

                            // ListViewItem 객체 생성 후 컬럼에 데이터 삽입(파일 명, 파일 경로, 파일 정보)
                            ListViewItem item = new ListViewItem();
                            item.SubItems.Add(result[0]);
                            item.SubItems.Add(result[1]);
                            item.SubItems.Add(result[2]);
                            item.SubItems.Add(result[3]);
                            item.SubItems.Add(result[4]);
                            item.SubItems.Add("0");

                            // listView에 ListViewItem 삽입
                            listView1.Items.Add(item);

                            // ------------------ listView 업데이트 종료
                            listView1.EndUpdate();
                            Application.DoEvents();

                            // 불러오고 난후 맨마지막 리스트 포커스해주기
                            // listView1.Items[listView1.Items.Count - 1].Selected = true;
                            listView1.Items[listView1.Items.Count - 1].Focused = true;
                        }
                        catch
                        {
                            MessageBox.Show("오류오류오류", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        #endregion

        #region Decodeing 함수(불러오기)
        private String[] F_BarcodeRead(String filePath, int BarcodeType)
        {
            //Decode 성공 여부(1: 성공)
            int intRtn = 0;

            //UTF-8 Encoding한 파일 담을 변수
            String result = null;

            //반환값 담을 배열
            String[] RtnResult = new string[5];

            byte[] utf8bytes = new byte[2000];

            StringBuilder sbPosition = new StringBuilder(1024);
            StringBuilder sbResult = new StringBuilder(1024);

            //Decoding 결과값 intRtn변수에 삽입
            intRtn = PACEBarcode_D.PACEBarcode_D_Auto(BarcodeType, filePath, sbPosition, utf8bytes);
            //intRtn = PACEBarcode_D.PACEBarcode_D_Region(PACEBarcode_D.PACE_BARCODETYPE_QRCODE, 483, 1263, 1200, 1900, filePath, sbPosition, sbResult);
            //intRtn = PACEBarcode_D.PACEBarcode_D_Auto(BarcodeType, filePath, sbPosition, sbResult);

            //intRtn가 1(성공)이면
            if (intRtn > 0)
            {
                //UTF-8 타입으로 Encoding, 공백 제거, 쓰레기값 제거 후 result변수에 삽입
                //decoding type이 byte[]일 때 사용
                result = Encoding.UTF8.GetString(utf8bytes).Trim('\0').Replace("�ܨͨϨ�", "");

                //decoding type이 StringBuilder일 때 사용
                //result = sbResult.ToString().Trim().Replace("ⓟⓐⓒⓔ", "");

                //result가 null이 아니면
                if (result != null)
                {
                    //RtnResult배열에 값 삽입(파일 명, 파일 경로, 파일 내용, 바코드 타입, decoding 결과)
                    //RtnResult배열에 값 삽입(인식여부, 파일명, 파일경로, 바코드 타입, decoding 결과)
                    RtnResult[0] = "x";
                    RtnResult[1] = Path.GetFileNameWithoutExtension(filePath);
                    RtnResult[2] = filePath; // result.ToString();
                }
            }
            else
                RtnResult = null;

            return RtnResult;
        }
        #endregion

        #region 전체인식
        private void btnRead_Click(object sender, EventArgs e)
        {
            String path = "";
            String[] result = null;

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                path = listView1.Items[i].SubItems[3].Text;

                result = F_BarcodeRead2(path, 0x0001);
                if (listView1.Items.Count > 0)
                {
                   if (listView1.Items[i].SubItems[1].Text.Equals("x"))
                   {
                        listView1.Items[i].SubItems[1].Text = result[0]; // o 표시
                        listView1.Items[i].SubItems[5].Text = result[4]; // 좌표값
                        if (result[3].Length < 11)
                        {
                            listView1.Items[i].SubItems[4].Text = result[3]; // 인식값
                        }
                        else
                        {
                            // 인식값의 길이가 10이상이면 길이로 나눠서 2개출력
                            string str = result[3];
                            string a = str.Substring(0, 10);
                            string b = str.Substring(10);
                            listView1.Items[i].SubItems[4].Text = a + "," + b;
                        }
                    }
                }
                Application.DoEvents();
            }
            MessageBox.Show("인식이 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        #endregion

        #region 선택인식
        private void btnRead2_Click(object sender, EventArgs e)
        {
            int cnt = 0;    //체크된 아이템 개수

            String path = "";
            String[] result = null;

            try
            {
                //checkBox 선택하고 버튼 클릭 시 정보 보여주기
                if (listView1.Items.Count > 0)
                {
                    //checkBox 체크된 개수 구하기
                    for (int i = 0; i <= listView1.Items.Count - 1; i++)
                    {
                        if (listView1.Items[i].Checked == true)
                        {
                            cnt++;
                        }
                    }

                    //체크된 아이템이 있을 경우 배열에 넣어서 보여줌
                    if (cnt == 1)
                    {
                        int _index = 0;    //배열 인덱스

                        //데이터를 배열에 삽입

                        if (listView1.FocusedItem.Checked == true)
                        {
                            if (listView1.FocusedItem.SubItems[3].Text.Equals(""))
                            {
                                path = listView1.FocusedItem.SubItems[3].Text;

                                result = F_BarcodeRead2(path, 0x0001);
                                if (listView1.Items.Count > 0)
                                {
                                    if (listView1.FocusedItem.SubItems[1].Text.Equals("x"))
                                    {
                                        listView1.FocusedItem.SubItems[1].Text = result[0];
                                        listView1.FocusedItem.SubItems[5].Text = result[4];
                                        if (result[3].Length < 11)
                                        {
                                            listView1.FocusedItem.SubItems[4].Text = result[3];
                                            MessageBox.Show("파일명 : " + listView1.FocusedItem.SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            string str = result[3];
                                            string a = str.Substring(0, 10);
                                            string b = str.Substring(10);
                                            listView1.FocusedItem.SubItems[4].Text = a + "," + b;
                                            MessageBox.Show("파일명 : " + listView1.FocusedItem.SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                path = listView1.FocusedItem.SubItems[3].Text;

                                result = F_BarcodeRead2(path, 0x0001);
                                if (listView1.Items.Count > 0)
                                {
                                    if (listView1.FocusedItem.SubItems[1].Text.Equals("x"))
                                    {
                                        listView1.FocusedItem.SubItems[1].Text = result[0];
                                        listView1.FocusedItem.SubItems[5].Text = result[4];
                                        if (result[3].Length < 11)
                                        {
                                            listView1.FocusedItem.SubItems[4].Text = result[3];
                                            MessageBox.Show("파일명 : " + listView1.FocusedItem.SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            string str = result[3];
                                            string a = str.Substring(0, 10);
                                            string b = str.Substring(10);
                                            listView1.FocusedItem.SubItems[4].Text = a + "," + b;
                                            MessageBox.Show("파일명 : " + listView1.FocusedItem.SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                            }
                            if (listView1.Items.Count != _index)
                                _index++;   //배열에 데이터를 넣은 뒤 다음 인덱스 값으로 바꿔줌
                        }

                    }
                    if (cnt > 1)
                    {
                        for (int i = 0; i <= listView1.Items.Count - 1; i++)
                        {
                            if (listView1.Items[i].Checked == true)
                            {
                                path = listView1.Items[i].SubItems[3].Text;

                                result = F_BarcodeRead2(path, 0x0001);
                                if (listView1.Items.Count > 0)
                                {
                                    if (listView1.Items[i].SubItems[1].Text.Equals("x"))
                                    {
                                        listView1.Items[i].SubItems[1].Text = result[0];
                                        listView1.Items[i].SubItems[5].Text = result[4];
                                        if (result[3].Length < 11)
                                        {
                                            listView1.Items[i].SubItems[4].Text = result[3];
                                            MessageBox.Show("파일명 : " + listView1.Items[i].SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            string str = result[3];
                                            string a = str.Substring(0, 10);
                                            string b = str.Substring(10);
                                            listView1.Items[i].SubItems[4].Text = a + "," + b;
                                            MessageBox.Show("파일명 : " + listView1.Items[i].SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    //체크된 아이템이 없을 경우 focus된 아이템을 보여줌
                    else
                    {
                        path = listView1.FocusedItem.SubItems[3].Text;

                        result = F_BarcodeRead2(path, 0x0001);
                        if (listView1.Items.Count > 0)
                        {
                            if (listView1.FocusedItem.SubItems[1].Text.Equals("x"))
                            {
                                listView1.FocusedItem.SubItems[1].Text = result[0];
                                listView1.FocusedItem.SubItems[5].Text = result[4];
                                if (result[3].Length < 11)
                                {
                                    listView1.FocusedItem.SubItems[4].Text = result[3];
                                    MessageBox.Show("파일명 : " + listView1.FocusedItem.SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    string str = result[3];
                                    string a = str.Substring(0, 10);
                                    string b = str.Substring(10);
                                    listView1.FocusedItem.SubItems[4].Text = a + "," + b;
                                    MessageBox.Show("파일명 : " + listView1.FocusedItem.SubItems[2].Text + " 데이터 인식 성공", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("선택한 항목이 없습니다.\n항목을 선택해주세요.", "Recognition_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Decodeing2 함수 (인식)
        private String[] F_BarcodeRead2(String filePath, int BarcodeType)
        {
            //Decode 성공 여부(1: 성공)
            int intRtn = 0;

            //UTF-8 Encoding한 파일 담을 변수
            String result = null;

            //반환값 담을 배열
            String[] RtnResult = new string[5];

            byte[] utf8bytes = new byte[2000];

            StringBuilder sbPosition = new StringBuilder(1024);
            StringBuilder sbResult = new StringBuilder(1024);

            //Decoding 결과값 intRtn변수에 삽입
            intRtn = PACEBarcode_D.PACEBarcode_D_Auto(BarcodeType, filePath, sbPosition, utf8bytes);
            //intRtn = PACEBarcode_D.PACEBarcode_D_Region(PACEBarcode_D.PACE_BARCODETYPE_QRCODE, 483, 1263, 1200, 1900, filePath, sbPosition, sbResult);
            //intRtn = PACEBarcode_D.PACEBarcode_D_Auto(BarcodeType, filePath, sbPosition, sbResult);

            //intRtn가 1(성공)이면
            if (intRtn > 0)
            {
                //UTF-8 타입으로 Encoding, 공백 제거, 쓰레기값 제거 후 result변수에 삽입
                //decoding type이 byte[]일 때 사용
                result = Encoding.UTF8.GetString(utf8bytes).Trim('\0').Replace("�ܨͨϨ�", "");

                //decoding type이 StringBuilder일 때 사용
                //result = sbResult.ToString().Trim().Replace("ⓟⓐⓒⓔ", "");

                //result가 null이 아니면
                if (result != null)
                {
                    //RtnResult배열에 값 삽입(파일 명, 파일 경로, 파일 내용, 바코드 타입, decoding 결과)
                    //RtnResult배열에 값 삽입(인식여부, 파일명, 파일경로, 바코드 타입, decoding 결과)
                    RtnResult[0] = "o";
                    RtnResult[1] = Path.GetFileNameWithoutExtension(filePath);
                    RtnResult[2] = filePath; // result.ToString();
                    RtnResult[3] = result;
                    RtnResult[4] = sbPosition.ToString();
                }
            }
            else
                RtnResult = null;

            return RtnResult;
        }
        #endregion

        #region 전체삭제
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("모두 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                listView1.Clear();
                RealEyeViewX.CloseDocument();          // 초기화
                RealEyeViewX2.CloseDocument();         // 초기화
                ListView_option();
            }
        }
        #endregion

        #region 선택삭제
        private void btnDel2_Click(object sender, EventArgs e)
        {
            int cnt = 0; // 체크개수 카운트
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                {
                    if (MessageBox.Show("선택항목을 삭제하시겠습니까?", "항목삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //select되있는 항목이 있으면 select 해제
                        if (listView1.SelectedItems.Count >= 1)
                        {
                            listView1.Items[0].Focused = false;
                            listView1.Items[0].Selected = false;
                            RealEyeViewX.CloseDocument();          // 초기화
                            RealEyeViewX2.CloseDocument();         // 초기화
                        }

                        //ListView에 Item이 있을 경우(없으면 수행 작업 x)
                        if (listView1.Items.Count > 0)
                        {
                            // 뒤에서 앞으로 roof를 돌며 체크된 항목을 제거
                            for (int j = listView1.Items.Count - 1; j >= 0; j--)
                            {
                                if (listView1.Items[j].Checked == true)
                                {
                                    listView1.Items[j].Remove();
                                    RealEyeViewX.CloseDocument();          // 초기화
                                    RealEyeViewX2.CloseDocument();         // 초기화
                                }
                            }
                        }
                        // ListView Item Index 재배열
                        for (int j = 0; j < listView1.Items.Count; j++)
                        {
                            listView1.Items[j].SubItems[0].Text = (j + 1).ToString();
                        }
                        cnt++;
                    }
                }
            }
            // 체크박스가 없을경우 선택된거만 삭제
            if (cnt == 0)
            {
                if (MessageBox.Show("선택항목을 삭제하시겠습니까?", "항목삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    listView1.FocusedItem.Remove();
                    listView1.FocusedItem.Focused = false;
                    RealEyeViewX.CloseDocument();          // 초기화
                    RealEyeViewX2.CloseDocument();         // 초기화
                }
            }
            // List 초기화 후 다시 입력
            listItem.Clear();
        }
        #endregion

        #region ListView 클릭 이벤트(클릭시 이미지보이게하기)
        // 여기에 잘라넣기 넣자
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // 클릭되어있는 아이템의 정보를 변수에 삽입
                String index = listView1.FocusedItem.SubItems[1].Text; // x
                String path = listView1.FocusedItem.SubItems[3].Text;  // 파일경로
                String info = listView1.FocusedItem.SubItems[4].Text;  // 인식값
                String type = listView1.FocusedItem.SubItems[5].Text;  // 좌표값

                RealEyeViewX.LoadDocument(RealEyeViewX.enumLoadType_LOCAL, RealEyeViewX.enumEncryptionType_NONE, path);
                RealEyeViewX2.CloseDocument();
                RealEyeViewX2.Update();

                // 선택한 리스트가 좌표값이 있다면
                if (!listView1.FocusedItem.SubItems[5].Text.Equals(""))
                {
                    string[] spstring = type.Split('|');     // | 단위로 나눠주기

                    int a = short.Parse(spstring[2]) - short.Parse(spstring[0]);     // 가로길이 구하기
                    int b = short.Parse(spstring[3]) - short.Parse(spstring[1]);     // 세로길이 구하기
                    string c = a.ToString();
                    string d = b.ToString();

                    string file_path = @"C:\바코드코드\"  + Path.GetFileNameWithoutExtension(path) + "_1" + Path.GetExtension(path);

                    // 이미지 경로가 있을 경우 저장되있는 이미지 출력
                    if (path != "")
                    {
                        FileInfo fi = new FileInfo(path);
                        if (fi.Exists)
                        {
                            if (listView1.FocusedItem.SubItems[1].Text.Equals(index))
                            {
                                if(spstring.Length < 10)
                                {
                                    // 바코드위치 형광표시하기 
                                    RealEyeViewX.AddShapeObject(1, RealEyeViewX.enumShapeType_ANNOTATION, path, RealEyeViewX.enumShapeStyle_RECTANGLE, short.Parse(spstring[0]), short.Parse(spstring[1]), short.Parse(c), short.Parse(d), (uint)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0)), (uint)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0)), 0, RealEyeViewX.enumLineDashStyle_SOLID, RealEyeViewX.enumShapeStyle_LINE, RealEyeViewX.enumShapeStyle_LINE, 70, 0, true);
                                    // 바코드위치 자르기
                                    RealEyeViewX.ClippingImage(0, path, 1, short.Parse(spstring[0]), short.Parse(spstring[1]), short.Parse(c), short.Parse(d), file_path, 2, 7, 1);
                                    RealEyeViewX2.LoadDocument(RealEyeViewX.enumLoadType_LOCAL, RealEyeViewX.enumEncryptionType_NONE, file_path);
                                }
                                else
                                {
                                    int a2 = short.Parse(spstring[10]) - short.Parse(spstring[8]);     // 가로길이 구하기
                                    int b2 = short.Parse(spstring[11]) - short.Parse(spstring[9]);     // 세로길이 구하기
                                    string c2 = a2.ToString();
                                    string d2 = b2.ToString();

                                    string file_path2 = @"C:\바코드코드\" + Path.GetFileNameWithoutExtension(path) + "_2" + Path.GetExtension(path);
                                    // 바코드위치 형광표시하기 
                                    RealEyeViewX.AddShapeObject(1, RealEyeViewX.enumShapeType_ANNOTATION, path, RealEyeViewX.enumShapeStyle_RECTANGLE, short.Parse(spstring[0]), short.Parse(spstring[1]), short.Parse(c), short.Parse(d), (uint)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0)), (uint)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0)), 0, RealEyeViewX.enumLineDashStyle_SOLID, RealEyeViewX.enumShapeStyle_LINE, RealEyeViewX.enumShapeStyle_LINE, 70, 0, true);
                                    RealEyeViewX.AddShapeObject(1, RealEyeViewX.enumShapeType_ANNOTATION, path, RealEyeViewX.enumShapeStyle_RECTANGLE, short.Parse(spstring[8]), short.Parse(spstring[9]), short.Parse(c2), short.Parse(d2), (uint)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0)), (uint)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0)), 0, RealEyeViewX.enumLineDashStyle_SOLID, RealEyeViewX.enumShapeStyle_LINE, RealEyeViewX.enumShapeStyle_LINE, 70, 0, true);

                                    // 바코드위치 자르기
                                    RealEyeViewX.ClippingImage(0, path, 1, short.Parse(spstring[0]), short.Parse(spstring[1]), short.Parse(c), short.Parse(d), file_path, 2, 7, 1);
                                    RealEyeViewX.ClippingImage(0, path, 1, short.Parse(spstring[8]), short.Parse(spstring[9]), short.Parse(c2), short.Parse(d2), file_path2, 2, 7, 1);
                                    RealEyeViewX2.LoadDocument(RealEyeViewX.enumLoadType_LOCAL, RealEyeViewX.enumEncryptionType_NONE, file_path);
                                    RealEyeViewX2.AppendDocument(0, 0,file_path2);
                                }
                            }
                            else
                            {
                                // 이미지 보이게하는 회사가 만든 프로그램
                                RealEyeViewX.LoadDocument(RealEyeViewX.enumLoadType_LOCAL, RealEyeViewX.enumEncryptionType_NONE, path);
                            }
                        }
                        else
                        {
                            string imagePath = Application.StartupPath + @"\" + "image";
                            RealEyeViewX.LoadDocument(RealEyeViewX.enumLoadType_LOCAL, RealEyeViewX.enumEncryptionType_NONE, path);
                            if (listView1.FocusedItem.SubItems[1].Text.Equals("o"))
                            {

                            }
                        }
                    }
                }

            }
            catch 
            {
               
            }
        }
        #endregion

        #region 오른쪽 이미지 불러오기 창
        private void Set_RealEyeView()
        {
            try
            {
                RealEyeViewX.EnableRenderingThumbnail = 1; // 썸네일 표시할지 여부
                RealEyeViewX.EnableRenderingPage = 1; // 페이지를 표시할지 여부
                RealEyeViewX.ThumbShowStyle = RealEyeViewX.enumThumbShowStyle_SINGLE_LINE; // 썸네일 보기 방식
                RealEyeViewX.ViewMode = RealEyeViewX.enumViewMode_BOTH; // 보기방식 - 썸네일,페이지 같이 보기
                RealEyeViewX.EnableAnnotation = 1; // 주석 사용 여부
                RealEyeViewX.VisibleAnnotation = 1; // 주석 표시 여부
                RealEyeViewX.ZoomMode = RealEyeViewX.enumZoomMode_FIT_PAGE; // 줌모드 설정 - 페이지를 뷰 윈도우 크기에 맞춤
                RealEyeViewX.EnableSaveAnnotation = 1; // 주석 저장 여부
                RealEyeViewX.VisibleToolBar = 1; // 기본 툴바 표시 여부
                RealEyeViewX.VisibleAnnotationToolBar = 1; // 주석 툴바 표시 여부
                RealEyeViewX.VisibleAnnotationPropToolBar = 1; // 주석 속성 툴바 표시 여부
                RealEyeViewX.VisibleThumbnailCaption = 1; // 썸네일의 캡션 표시 여부
                RealEyeViewX.LoadThumbStyle = RealEyeViewX.enumLoadThumbStyle_SYNC; // 썸네일 로드 방식 - 동기화
                RealEyeViewX.ThumbSelectStyle = RealEyeViewX.enumThumbSelectStyle_SINGLE; // 썸네일 선택 방식 - 단일 선택
                RealEyeViewX.VisibleStatusBar = 1; // 상태바 표시 여부
                RealEyeViewX.EnablePrintAnnotation = 1; // 주석 인쇄 여부
                RealEyeViewX.EnablePrintDocument = 1; // 로드된 문서의 인쇄 여부
                RealEyeViewX.EnablePrintWatermarkImage = 1; // 워터마크 이미지 인쇄 여부
                RealEyeViewX.VisibleWatermarkImage = 1; // 워터마크 이미지 표시 여부
                RealEyeViewX.EnableAnnotationChangedEvent = 1; // 주석 편집 이벤트 발생 여부
                RealEyeViewX.MagnifierZoomRatio = 200; // 돋보기의 확대 비율 설정
                RealEyeViewX.EnablePostInputMethodMessageEvent = 1; // 키보드/마우스 입력 이벤트 발생 여부
                RealEyeViewX.EnableAnnotCustomPopupMenuEvent = 0; // 주석 팝업 메뉴 나타나기 전 이벤트 발생 여부
                RealEyeViewX.PageMarginHeight = 0; // 페이지의 여백 높이
                RealEyeViewX.PageMarginWidth = 0; // 페이지의 여백 너비
                RealEyeViewX.EnableKeepScrollPos = 1; // 페이지 이동시 스크롤 위치 고정 여부
                RealEyeViewX.EnableKeepAnnotDrawTool = 1; // 주석 그리기 툴 유지 여부
                RealEyeViewX.BackgroundColor = Color.Gainsboro; // 페이지 뷰 윈도우의 배경색
                RealEyeViewX.ThumbWindowBackgroundColor = Color.White; // 썸네일 윈도우의 배경색
                RealEyeViewX.ThumbWindowWidth = 120; // 썸네일 윈도우의 폭
                RealEyeViewX.ThumbHeight = 90; // 썸네일 높이
                RealEyeViewX.ThumbWidth = 90; // 썸네일 너비
                RealEyeViewX.MainToolBarSize = RealEyeViewX.enumToolBarSize_SMALL; // 메인 툴바 사이즈
                RealEyeViewX.AnnotToolBarSize = RealEyeViewX.enumToolBarSize_SMALL; // 주석 툴바 사이즈
                RealEyeViewX.AnnotPropToolBarSize = RealEyeViewX.enumToolBarSize_SMALL; // 주석 속성 툴바의 사이즈
                RealEyeViewX.EnableDragThumbnail = 0; // 썸네일 드래그
                RealEyeViewX.EnableSupportDropFile = 0; // 문서파일 드래그 & 드롭하여 문서열기 기능 지원
                RealEyeViewX.EnableAppendOnDropFile = 0;  // 문서파일 드래그 & 드롭시 문서 추가

            }
            catch (Exception ex)
            {
                MessageBox.Show("- 오류 타입: " + ex.GetType() +
                                "\n\r\n- 오류 내용: " + ex.Message, "InitRealEyeViewX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 아래쪽 이미지 불러오기 창
        private void Set_RealEyeView2()
        {
            try
            {
                RealEyeViewX2.EnableRenderingThumbnail = 1; // 썸네일 표시할지 여부
                RealEyeViewX2.EnableRenderingPage = 1; // 페이지를 표시할지 여부
                RealEyeViewX2.ThumbShowStyle = RealEyeViewX.enumThumbShowStyle_SINGLE_LINE; // 썸네일 보기 방식
                RealEyeViewX2.ViewMode = RealEyeViewX.enumViewMode_BOTH; // 보기방식 - 썸네일,페이지 같이 보기
                RealEyeViewX2.EnableAnnotation = 1; // 주석 사용 여부
                RealEyeViewX2.VisibleAnnotation = 1; // 주석 표시 여부
                RealEyeViewX2.ZoomMode = RealEyeViewX.enumZoomMode_FIT_PAGE; // 줌모드 설정 - 페이지를 뷰 윈도우 크기에 맞춤
                RealEyeViewX2.EnableSaveAnnotation = 1; // 주석 저장 여부
                RealEyeViewX2.VisibleToolBar = 1; // 기본 툴바 표시 여부
                RealEyeViewX2.VisibleAnnotationToolBar = 1; // 주석 툴바 표시 여부
                RealEyeViewX2.VisibleAnnotationPropToolBar = 1; // 주석 속성 툴바 표시 여부
                RealEyeViewX2.VisibleThumbnailCaption = 1; // 썸네일의 캡션 표시 여부
                RealEyeViewX2.LoadThumbStyle = RealEyeViewX.enumLoadThumbStyle_SYNC; // 썸네일 로드 방식 - 동기화
                RealEyeViewX2.ThumbSelectStyle = RealEyeViewX.enumThumbSelectStyle_SINGLE; // 썸네일 선택 방식 - 단일 선택
                RealEyeViewX2.VisibleStatusBar = 1; // 상태바 표시 여부
                RealEyeViewX2.EnablePrintAnnotation = 1; // 주석 인쇄 여부
                RealEyeViewX2.EnablePrintDocument = 1; // 로드된 문서의 인쇄 여부
                RealEyeViewX2.EnablePrintWatermarkImage = 1; // 워터마크 이미지 인쇄 여부
                RealEyeViewX2.VisibleWatermarkImage = 1; // 워터마크 이미지 표시 여부
                RealEyeViewX2.EnableAnnotationChangedEvent = 1; // 주석 편집 이벤트 발생 여부
                RealEyeViewX2.MagnifierZoomRatio = 200; // 돋보기의 확대 비율 설정
                RealEyeViewX2.EnablePostInputMethodMessageEvent = 1; // 키보드/마우스 입력 이벤트 발생 여부
                RealEyeViewX2.EnableAnnotCustomPopupMenuEvent = 0; // 주석 팝업 메뉴 나타나기 전 이벤트 발생 여부
                RealEyeViewX2.PageMarginHeight = 0; // 페이지의 여백 높이
                RealEyeViewX2.PageMarginWidth = 0; // 페이지의 여백 너비
                RealEyeViewX2.EnableKeepScrollPos = 1; // 페이지 이동시 스크롤 위치 고정 여부
                RealEyeViewX2.EnableKeepAnnotDrawTool = 1; // 주석 그리기 툴 유지 여부
                RealEyeViewX2.BackgroundColor = Color.Gainsboro; // 페이지 뷰 윈도우의 배경색
                RealEyeViewX2.ThumbWindowBackgroundColor = Color.White; // 썸네일 윈도우의 배경색
                RealEyeViewX2.ThumbWindowWidth = 120; // 썸네일 윈도우의 폭
                RealEyeViewX2.ThumbHeight = 90; // 썸네일 높이
                RealEyeViewX2.ThumbWidth = 90; // 썸네일 너비
                RealEyeViewX2.MainToolBarSize = RealEyeViewX.enumToolBarSize_SMALL; // 메인 툴바 사이즈
                RealEyeViewX2.AnnotToolBarSize = RealEyeViewX.enumToolBarSize_SMALL; // 주석 툴바 사이즈
                RealEyeViewX2.AnnotPropToolBarSize = RealEyeViewX.enumToolBarSize_SMALL; // 주석 속성 툴바의 사이즈
                RealEyeViewX2.EnableDragThumbnail = 0; // 썸네일 드래그
                RealEyeViewX2.EnableSupportDropFile = 0; // 문서파일 드래그 & 드롭하여 문서열기 기능 지원
                RealEyeViewX2.EnableAppendOnDropFile = 0;  // 문서파일 드래그 & 드롭시 문서 추가

            }
            catch (Exception ex)
            {
                MessageBox.Show("- 오류 타입: " + ex.GetType() +
                                "\n\r\n- 오류 내용: " + ex.Message, "InitRealEyeViewX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}

