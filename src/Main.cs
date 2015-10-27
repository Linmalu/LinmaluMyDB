using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LinmaluMyDB
{
    public partial class Main : Form
    {
        private const string version = "0.2";
        public LinmaluDB db { get; set; }
        public Point DRAG_HANDLE_SIZE { get; private set; }

        public Main()
        {
            InitializeComponent();
            Text += "_" + version;
            menuLogout.Enabled = menuRefresh.Enabled = false;
        }

        public void clear()
        {
            menuLogout.Enabled = menuRefresh.Enabled = true;
            treeView1.Nodes.Clear();
            listView1.Items.Clear();
            listView1.Columns.Clear();
            listView2.Clear();
            foreach (string database in db.showDatabases())
            {
                TreeNode tn = new TreeNode(database);
                foreach (string table in db.showTables(database))
                {
                    tn.Nodes.Add(table);
                }
                treeView1.Nodes.Add(tn);
            }
        }

        private void openLogion()
        {
            new Login(this).ShowDialog();
        }

        private void LinmaluMyDB_Shown(object sender, EventArgs e)
        {
            openLogion();
        }

        private void refreshListView()
        {
            TreeNode tn = treeView1.SelectedNode;
            listView1.Clear();
            listView2.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
            if (tn.Parent != null)
            {
                List<string> columns = new List<string>();
                dataGridView2.Columns.Add("where", "where");
                foreach(string column in db.showColumnsType(tn.Parent.Text, tn.Text))
                {
                    listView2.Columns.Add(column);
                }
                foreach (List<string> column in db.showColumns(tn.Parent.Text, tn.Text))
                {
                    listView1.Columns.Add(column[0]);
                    listView2.Items.Add(new ListViewItem(column.ToArray()));
                    columns.Add(column[0]);
                    dataGridView1.Columns.Add(column[0], column[0]);
                    dataGridView2.Columns.Add(column[0], column[0]);
                }
                foreach (List<string> list in db.showDatas(tn.Parent.Text, tn.Text, columns.ToArray()))
                {
                    listView1.Items.Add(new ListViewItem(list.ToArray()));
                }
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridView2.Columns[0].Visible = false;
            }
            else
            {
                listView1.Columns.Add("테이블");
                foreach (string table in db.showTables(tn.Text))
                {
                    listView1.Items.Add(table);
                }
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            refreshListView();
        }

        private string getWhere()
        {
            string where = "WHERE ";
            bool run = false;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                where += run ? " OR (" : "(";
                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    if (!item.SubItems[i].Text.StartsWith("System."))
                    {
                        where += (i == 0 ? "" : " AND ") + listView1.Columns[i].Text + " = '" + item.SubItems[i].Text + "'";
                    }
                }
                where += ")";
                run = true;
            }
            return run ? where : "";
        }

        private void menuInsert_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                List<string> list = new List<string>();

                string where = "WHERE ";
                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    if (!item.SubItems[i].Text.StartsWith("System."))
                    {
                        where += (i == 0 ? "" : " AND ") + listView1.Columns[i].Text + " = '" + item.SubItems[i].Text + "'";
                    }
                }
                list.Add(where);
                foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                {
                    list.Add(sub.Text);
                }
                dataGridView2.Rows.Add(list.ToArray());
            }
            dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "삭제될 갯수 : " + db.countDatas(treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text, getWhere()), "삭제확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MessageBox.Show(this, db.deleteDatas(treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text, getWhere()) + "개가 삭제되었습니다.", "삭제완료");
                refreshListView();
            }
        }

        private void menuDBInsert_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Index < dataGridView1.Rows.Count - 1)
                {
                    List<string> list = new List<string>();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        list.Add(cell.Value as string);
                    }
                    count += db.insertDatas(treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text, list.ToArray());
                }
            }
            MessageBox.Show(this, count + "개가 추가되었습니다.", "추가완료");
            refreshListView();
            tabControl1.SelectedIndex = 0;
        }

        private void menuDBUpdate_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Index < dataGridView2.Rows.Count - 1)
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        map.Add(dataGridView2.Columns[i].HeaderText, row.Cells[i].Value as string);
                    }
                    count += db.updateDatas(treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text, map, row.Cells[0].Value as string);
                }
            }
            MessageBox.Show(this, count + "개가 수정되었습니다.", "수정완료");
            refreshListView();
            tabControl1.SelectedIndex = 0;
        }

        private void menuDBDelete_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> list = new List<DataGridViewRow>();
            DataGridView gv = tabControl1.SelectedIndex == 2 ? dataGridView1 : dataGridView2;
            foreach (DataGridViewCell cell in gv.SelectedCells)
            {
                if (cell.RowIndex < gv.Rows.Count - 1)
                {
                    list.Add(gv.Rows[cell.RowIndex]);
                }
            }
            foreach (DataGridViewRow row in list)
            {
                if (gv.Rows.Contains(row))
                {
                    gv.Rows.Remove(row);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                e.Cancel = true;
            }
            else if (treeView1.SelectedNode.Parent == null)
            {
                menuInsert.Enabled = menuUpdate.Enabled = menuDelete.Enabled = false;
            }
            else if (listView1.SelectedItems.Count == 0)
            {
                menuInsert.Enabled = true;
                menuUpdate.Enabled = menuDelete.Enabled = false;
            }
            else
            {
                menuInsert.Enabled = menuUpdate.Enabled = menuDelete.Enabled = true;
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                e.Cancel = true;
            }
            else if (treeView1.SelectedNode.Parent == null)
            {
                menuDBInsert.Enabled = menuDBUpdate.Enabled = menuDBDelete.Enabled = false;
            }
            else if ((tabControl1.SelectedIndex == 2 && dataGridView1.Rows.Count <= 1) || (tabControl1.SelectedIndex == 3 && dataGridView2.Rows.Count <= 1))
            {
                menuDBInsert.Enabled = menuDBUpdate.Enabled = menuDBDelete.Enabled = false;
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                menuDBInsert.Enabled = true;
                menuDBUpdate.Enabled = false;
                menuDBDelete.Enabled = true;
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                menuDBInsert.Enabled = false;
                menuDBUpdate.Enabled = true;
                menuDBDelete.Enabled = true;
            }
        }

        private void menuLogin_Click(object sender, EventArgs e)
        {
            openLogion();
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            menuLogout.Enabled = menuRefresh.Enabled = false;
            treeView1.Nodes.Clear();
            listView1.Clear();
            listView2.Items.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
        }

        private void menuRefresh_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void Linmalu_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://blog.linmalu.com");
        }
    }
}
