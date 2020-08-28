using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Util.Basics
{
    /// <summary>
    /// 合并单元格的方向
    /// </summary>
    public enum MergeDirection
    {
        /// <summary>
        /// 横向
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// 纵向
        /// </summary>
        Vertical = 1,
    }
    /// <summary>
    /// 合并单元格操作类
    /// </summary>
    public class MergeCells
    {
        private static SortedList rowSpan = new SortedList();//取得需要重新绘制的单元格
        private static string rowValue = "";//重新绘制的文本框内容

        #region  单元格绘制合并
        /// <summary>
        /// 
        /// DataGridView合并单元格
        /// </summary>
        /// <param name="dgv">要绘制的DataGridview</param>
        /// <param name="cellArgs">绘制单元格的参数（DataGridview的CellPainting事件中参数）</param>
        /// <param name="minColIndex">起始单元格在DataGridView中的索引号</param>
        /// <param name="maxColIndex">结束单元格在DataGridView中的索引号</param>
        /// <param name="kind">合并单元格的方向</param>
        public static void MerageCells(DataGridView dgv, DataGridViewCellPaintingEventArgs cellArgs, int minColIndex, int maxColIndex, MergeDirection kind)
        {
            int Index = kind == 0 ? cellArgs.ColumnIndex : cellArgs.RowIndex;
            if (Index < minColIndex || Index > maxColIndex) return;

            Rectangle rect = new Rectangle();
            using (Brush gridBrush = new SolidBrush(dgv.GridColor),
                backColorBrush = new SolidBrush(cellArgs.CellStyle.BackColor))
            {
                //抹去原来的cell背景
                cellArgs.Graphics.FillRectangle(backColorBrush, cellArgs.CellBounds);
            }
            cellArgs.Handled = true;

            if (rowSpan[Index] == null)
            {
                //首先判断当前单元格是不是需要重绘的单元格
                //保留此单元格的信息，并抹去此单元格的背景
                rect.X = cellArgs.CellBounds.X;
                rect.Y = cellArgs.CellBounds.Y;
                rect.Width = cellArgs.CellBounds.Width;
                rect.Height = cellArgs.CellBounds.Height;

                if (Index == minColIndex)
                    rowValue = cellArgs.Value == null ? "" : cellArgs.Value.ToString();
                rowSpan.Add(Index, rect);
                if (Index != maxColIndex)
                    return;
                MeragePrint(dgv, cellArgs, minColIndex, maxColIndex, kind);
            }
            else
            {
                if (Index == minColIndex)
                    rowValue = cellArgs.Value == null ? "" : cellArgs.Value.ToString();
                IsPostMerage(dgv, cellArgs, minColIndex, maxColIndex, kind);
            }
        }

        /// <summary>
        /// 不是初次单元格绘制
        /// </summary>
        /// <param name="dgv">要绘制的DataGridview</param>
        /// <param name="cellArgs">绘制单元格的参数（DataGridview的CellPainting事件中参数）</param>
        /// <param name="minColIndex">起始单元格在DataGridView中的索引号</param>
        /// <param name="maxColIndex">结束单元格在DataGridView中的索引号</param>
        /// <param name="kind">合并单元格的方向</param>
        public static void IsPostMerage(DataGridView dgv, DataGridViewCellPaintingEventArgs cellArgs, int minColIndex, int maxColIndex, MergeDirection kind)
        {
            int Index = kind == 0 ? cellArgs.ColumnIndex : cellArgs.RowIndex;
            //比较单元是否有变化
            Rectangle rectArgs = (Rectangle)rowSpan[Index];
            if (rectArgs.X != cellArgs.CellBounds.X || rectArgs.Y != cellArgs.CellBounds.Y || rectArgs.Width != cellArgs.CellBounds.Width || rectArgs.Height != cellArgs.CellBounds.Height)
            {
                rectArgs.X = cellArgs.CellBounds.X;
                rectArgs.Y = cellArgs.CellBounds.Y;
                rectArgs.Width = cellArgs.CellBounds.Width;
                rectArgs.Height = cellArgs.CellBounds.Height;
                rowSpan[Index] = rectArgs;
            }
            if (Index == maxColIndex)
                MeragePrint(dgv, cellArgs, minColIndex, maxColIndex, kind);
        }

        /// <summary>
        /// 绘制单元格
        /// </summary>
        /// <param name="dgv">要绘制的DataGridview</param>
        /// <param name="cellArgs">绘制单元格的参数（DataGridview的CellPainting事件中参数）</param>
        /// <param name="minColIndex">起始单元格在DataGridView中的索引号</param>
        /// <param name="maxColIndex">结束单元格在DataGridView中的索引号</param>
        /// <param name="kind">合并单元格的方向</param>
        private static void MeragePrint(DataGridView dgv, DataGridViewCellPaintingEventArgs cellArgs, int minColIndex, int maxColIndex, MergeDirection kind)
        {
            int length = 0;
            for (int i = minColIndex; i <= maxColIndex; i++)
            {
                length += kind == 0 ? ((Rectangle)rowSpan[i]).Width : ((Rectangle)rowSpan[i]).Height;
            }
            int width = kind == 0 ? length : cellArgs.CellBounds.Width;//合并后单元格总宽度
            int height = kind == 0 ? cellArgs.CellBounds.Height : length;//合并后单元格总高度
            Rectangle rectBegin = (Rectangle)rowSpan[minColIndex];//合并第一个单元格的位置信息
            Rectangle rectEnd = (Rectangle)rowSpan[maxColIndex];//合并最后一个单元格的位置信息

            //合并单元格的位置信息
            Rectangle reBounds = new Rectangle();
            reBounds.X = rectBegin.X;
            reBounds.Y = rectBegin.Y;
            reBounds.Width = width - 1;
            reBounds.Height = height - 1;

            using (Brush gridBrush = new SolidBrush(dgv.GridColor),
                         backColorBrush = new SolidBrush(cellArgs.CellStyle.BackColor))
            {
                using (Pen gridLinePen = new Pen(gridBrush))
                {
                    // 画出上下两条边线，左右边线无
                    Point blPoint = new Point(kind == 0 ? rectBegin.Left : rectEnd.Left, kind == 0 ? rectBegin.Bottom - 1 : rectEnd.Bottom - 1);//底线左边位置
                    Point brPoint = new Point(rectEnd.Right - 1, rectEnd.Bottom - 1);//底线右边位置
                    cellArgs.Graphics.DrawLine(gridLinePen, blPoint, brPoint);//下边线

                    Point tlPoint = new Point(rectBegin.Left, rectBegin.Top == 0 ? rectBegin.Top : rectBegin.Top - 1);//上边线左边位置
                    Point trPoint = new Point(kind == 0 ? rectEnd.Right - 1 : rectBegin.Right - 1, kind == 0 ? (rectEnd.Top == 0 ? rectEnd.Top : rectEnd.Top - 1) : (rectBegin.Top == 0 ? rectBegin.Top : rectBegin.Top - 1));//上边线右边位置
                    cellArgs.Graphics.DrawLine(gridLinePen, tlPoint, trPoint); //上边线

                    Point ltPoint = new Point(rectBegin.Left == 0 ? rectBegin.Left : rectBegin.Left - 1, rectBegin.Top);//左边线顶部位置
                    Point lbPoint = new Point(kind == 0 ? (rectBegin.Left == 0 ? rectBegin.Left : rectBegin.Left - 1) : (rectEnd.Left == 0 ? rectEnd.Left : rectEnd.Left - 1), kind == 0 ? rectBegin.Bottom - 1 : rectEnd.Bottom - 1);//左边线底部位置
                    cellArgs.Graphics.DrawLine(gridLinePen, ltPoint, lbPoint); //左边线

                    Point rtPoint = new Point(kind == 0 ? rectEnd.Right - 1 : rectBegin.Right - 1, kind == 0 ? rectEnd.Top : rectBegin.Top);//右边线顶部位置
                    Point rbPoint = new Point(rectEnd.Right - 1, rectEnd.Bottom - 1);//右边线底部位置
                    cellArgs.Graphics.DrawLine(gridLinePen, rtPoint, rbPoint); //右边线

                    //计算绘制字符串的位置
                    SizeF sf = cellArgs.Graphics.MeasureString(rowValue, cellArgs.CellStyle.Font);
                    //float lstr = (width - sf.Width) / 2;      //水平居中
                    float lstr = 0;                             //左对齐
                    float rstr = (height - sf.Height) / 2;      //垂直居中
                    //画出文本框
                    if (rowValue != "")
                        cellArgs.Graphics.DrawString(rowValue, cellArgs.CellStyle.Font, new SolidBrush(cellArgs.CellStyle.ForeColor), rectBegin.Left + lstr, rectBegin.Top + rstr, StringFormat.GenericDefault);
                }
                cellArgs.Handled = true;
            }
        }
        #endregion
    }
}
