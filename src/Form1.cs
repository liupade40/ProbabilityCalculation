using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProbabilityCalculation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int count = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            //生成抽奖列表
            List<Thing> things = new List<Thing>();
            //生成随机抽奖列表
            List<Thing> randomThings = new List<Thing>();
            //界面上的抽奖数据
            List<Thing> list = new List<Thing>();
            try
            {
                //抽奖列表数据
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    var thingName = dataGridView1.Rows[i].Cells[0].Value == null ? "" : dataGridView1.Rows[i].Cells[0].Value.ToString();
                    decimal.TryParse(dataGridView1.Rows[i].Cells[1].Value?.ToString(), out decimal probability);
                    decimal.TryParse(dataGridView1.Rows[i].Cells[2].Value?.ToString(), out decimal value);
                    bool.TryParse(dataGridView1.Rows[i].Cells[3].Value?.ToString(), out bool isCheck);
                    list.Add(new Thing
                    {
                        ThingName = thingName ?? "",
                        Probability = probability,
                        Valuse = value,
                        IsCheck = isCheck,
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入正确的抽奖数据");
                return;
            }
            if (list.Any(x => x.Probability == 0 && x.IsCheck))
            {
                MessageBox.Show("暂停的抽奖概率不能为0");
                return;
            }
            if (list.All(x => !x.IsCheck))
            {
                MessageBox.Show("请选择抽中暂停的奖品");
                return;
            }
            var min = list.Where(x => x.Probability > 0).Select(x => x.Probability).Min();
            var d = 0;
            var b = 1;
            //最小的概率是否是小数，小数的话按10的倍数转成整数
            while (true && min.ToString().Contains("."))
            {
                b = b * 10;
                if ((min * d).ToString().Contains(".00"))
                {
                    d = b;
                    break;
                }
            }
            //根据概率生成对应的奖品数量
            foreach (var item in list)
            {
                for (int i = 0; i < item.Probability * d; i++)
                {
                    things.Add(item);
                }
            }
            //随机打乱奖品顺序
            while (things.Count > 0)
            {
                Random r = new Random();
                var index = r.Next(0, things.Count - 1);
                randomThings.Add(things[index]);
                things.RemoveAt(index);
            }
            richTextBox1.Clear();
            while (true)
            {
                count += 1;
                Random random = new Random();
                int i = random.Next(0, randomThings.Count - 1);
                var model = randomThings[i];
                richTextBox1.Text = richTextBox1.Text.Insert(0, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {count}-{i}-{model.ThingName}\n");
                if (model.IsCheck)//代表抽中自己想要的奖品
                {
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        if (decimal.TryParse(textBox1.Text, out decimal price))
                        {
                            var cost = count * price;
                            richTextBox2.Text = richTextBox2.Text.Insert(0, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 总共抽取了{count}次抽中了{model.ThingName},花费{cost},{(model.Valuse - cost <= 0 ? "亏" : "赚")}了{Math.Abs(model.Valuse - cost)}。\n");
                        }
                        else
                        {
                            richTextBox2.Text = richTextBox2.Text.Insert(0, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 总共抽取了{count}次抽中了{model.ThingName}。\n");
                        }
                    }
                    else
                    {
                        richTextBox2.Text = richTextBox2.Text.Insert(0, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 总共抽取了{count}次抽中了{model.ThingName}。\n");
                    }
                    count = 0;
                    break;
                } 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
        }
    }

    public class Thing
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string? ThingName { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public decimal Probability { get; set; }

        /// <summary>
        /// 概率
        /// </summary>
        public int ProbabilityNumber { get; set; }
        /// <summary>
        /// 价值
        /// </summary>
        public decimal Valuse { get; set; }
        /// <summary>
        /// 抽中是否暂停
        /// </summary>
        public bool IsCheck { get; set; }
    }
}
