using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ILPatcher.Interface.General
{
	class GridLineManager
	{
		private bool useGlobalLayout = false;

		private static int globalElementDistance = 5;
		private int localElementDistance = 20;
		public int ElementDistance
		{
			get { return useGlobalLayout ? globalElementDistance : localElementDistance; }
			set { if (useGlobalLayout)  globalElementDistance = value; else  localElementDistance = value; }
		}

		private Control parent;
		private List<GridLine> gridLinesList;
		private List<LayoutElement> elementList;

		public GridLineManager(Control parent, bool globalLayout)
		{
			this.parent = parent;
			this.useGlobalLayout = globalLayout;
			gridLinesList = new List<GridLine>();
			elementList = new List<LayoutElement>();
			parent.Resize += parent_Resize;
		}

		void parent_Resize(object sender, EventArgs e)
		{
			GenerateAxisPositionValues(elementList, parent.Height);
			parent.SuspendLayout();
			foreach (var element in gridLinesList)
			{
				GenerateAxisPositionValues(element.elementList, parent.Width);
				element.MatchContent();
			}
			parent.ResumeLayout();
		}

		private void GenerateAxisPositionValues(List<LayoutElement> listToSize, int targetSize)
		{
			var fixedElements = new List<LayoutElement>();
			var prefnsElements = new List<LayoutElement>();
			var fillElements = new List<LayoutElement>();

			int minFixedSum = 0;
			int minPrefnsSum = 0;
			int minFillSum = 0;

			int prefnsSum = 0;
			int maxPrefnsElement = 0;

			#region sort up
			foreach (var element in listToSize)
			{
				switch (element.arrangementType)
				{
				case ArrangementType.Fixed:
					minFixedSum += element.minValue;
					fixedElements.Add(element);
					break;
				case ArrangementType.PreferredAndShrink:
					minPrefnsSum += element.minValue;
					prefnsSum += element.prefValue;
					maxPrefnsElement = Math.Max(element.prefValue, maxPrefnsElement);
					prefnsElements.Add(element);
					break;
				case ArrangementType.Fill:
					minFillSum += element.minValue;
					fillElements.Add(element);
					break;
				case ArrangementType.None:
				default:
					break;
				}
			}
			#endregion

			int spaceCount = (listToSize.Count + 1);
			int neededSpaceSum = ElementDistance * spaceCount;

			int minSizePossible = minFixedSum + minPrefnsSum + minFillSum;
			int minSizeSpacedPossible = minSizePossible + neededSpaceSum;
			int minSizeWanted = minFixedSum + prefnsSum + minFillSum + neededSpaceSum;

			if (targetSize < minSizePossible) // 0 < x < minHeightPossible
			{
				// do nothing or shrink all proportianally
				return;
			}
			else if (targetSize < minSizeSpacedPossible) // minHeightPossible < x < minHeightSpacedPossible
			{
				int spaceRemainder = targetSize - minSizePossible;
				int pxPerSpace = spaceRemainder / spaceCount;

				int location = pxPerSpace;
				foreach (var element in listToSize)
				{
					element.curSize = element.minValue;
					element.curPosition = location;

					location += element.curSize + pxPerSpace;
				}
			}
			else if (targetSize < minSizeWanted) // minHeightSpacedPossible < x < minHeightWanted
			{
				int prefnsRemainder = targetSize - minSizeSpacedPossible;
				Rational ratSum = GetFractionSum(prefnsElements, maxPrefnsElement);
				Rational fragmentPiece = ratSum.IsZero() ? new Rational(prefnsRemainder, 1) : prefnsRemainder / ratSum;

				int location = ElementDistance;
				foreach (var element in listToSize)
				{
					if (element.arrangementType == ArrangementType.PreferredAndShrink)
						element.curSize = element.minValue + (int)(fragmentPiece / element.fractionOfMax);
					else
						element.curSize = element.minValue;
					element.curPosition = location;
					location += element.curSize + ElementDistance;
				}
			}
			else // minHeightWanted < x 
			{
				int pixelRemaining = targetSize - minSizeWanted;
				int extraPerElement = fillElements.Count == 0 ? 0 : pixelRemaining / fillElements.Count;

				int location = ElementDistance;
				foreach (var element in listToSize)
				{
					switch (element.arrangementType)
					{
					case ArrangementType.Fixed: element.curSize = element.minValue; break;
					case ArrangementType.PreferredAndShrink: element.curSize = element.prefValue; break;
					case ArrangementType.Fill: element.curSize = element.minValue + extraPerElement; break;
					}
					element.curPosition = location;
					location += element.curSize + ElementDistance;
				}
			}
		}

		private Rational GetFractionSum(List<LayoutElement> fractList, int ofMaximum)
		{
			Rational ratSum = new Rational(0, 1);
			foreach (var element in fractList)
			{
				element.fractionOfMax = new Rational(element.prefValue, ofMaximum);
				ratSum += element.fractionOfMax;
			}
			return ratSum;
		}

		// Add new Lines

		public int AddLineFixed(int height)
		{
			return AddLineInternal(ArrangementType.Fixed, height, -1);
		}

		public int AddLineStrechable(int minHeight, int maxHeight)
		{
			return AddLineInternal(ArrangementType.PreferredAndShrink, minHeight, maxHeight);
		}

		public int AddLineFilling(int minHeight)
		{
			return AddLineInternal(ArrangementType.Fill, minHeight, -1);
		}

		private int AddLineInternal(ArrangementType arrType, int minHeight, int maxHeight)
		{
			var elem = new GridLine(arrType, minHeight);
			elem.prefValue = maxHeight;
			gridLinesList.Add(elem);
			elementList.Add(elem);
			return gridLinesList.Count - 1;
		}

		// Add elements

		public void AddElementFixed(int line, Control child, int width)
		{
			AddElementInternal(line, child, ArrangementType.Fixed, width, -1);
		}

		public void AddElementStrechable(int line, Control child, int minWidth, int maxHeight)
		{
			AddElementInternal(line, child, ArrangementType.PreferredAndShrink, minWidth, maxHeight);
		}

		public void AddElementFilling(int line, Control child, int minWidth)
		{
			AddElementInternal(line, child, ArrangementType.Fill, minWidth, -1);
		}

		private void AddElementInternal(int line, Control child, ArrangementType arrType, int minWidth, int maxHeight)
		{
			parent.Controls.Add(child);
			var elem = new GridElement(child, arrType, minWidth);
			elem.prefValue = maxHeight;
			gridLinesList[line].AddElement(elem);
		}

		// Class structures

		private class GridLine : LayoutElement
		{
			public List<GridElement> gridElementList;
			public List<LayoutElement> elementList;

			public GridLine(ArrangementType arrTyp, int minVal)
				: base(arrTyp, minVal)
			{
				gridElementList = new List<GridElement>();
				elementList = new List<LayoutElement>();
			}

			public override void MatchContent()
			{
				foreach (var element in gridElementList)
				{
					Control ctrl = element.control;
					ctrl.Top = curPosition;
					ctrl.Height = curSize;
					element.MatchContent();
				}
			}

			public void AddElement(GridElement element)
			{
				gridElementList.Add(element);
				elementList.Add(element);
			}

			public void RemoveElement(GridElement element)
			{
				gridElementList.Remove(element);
				elementList.Remove(element);
			}

			public GridElement GetElementAt(int index)
			{
				return gridElementList[index];
			}
		}

		private class GridElement : LayoutElement
		{
			public Control control;

			public GridElement(Control control, ArrangementType arrTyp, int minVal)
				: base(arrTyp, minVal)
			{
				this.control = control;
			}

			public override void MatchContent()
			{
				control.Left = curPosition;
				control.Width = curSize;
			}
		}

		private abstract class LayoutElement
		{
			public ArrangementType arrangementType;
			public int minValue;
			public int prefValue;

			public int curPosition;
			public int curSize;
			public Rational fractionOfMax;

			protected LayoutElement(ArrangementType arrTyp, int minVal)
			{
				arrangementType = arrTyp;
				minValue = minVal;
			}

			public abstract void MatchContent();
		}

		enum ArrangementType
		{
			None,
			Fixed,
			PreferredAndShrink,
			Fill,
		}

		private class Rational
		{
			int numerator;
			int denominator;

			public static readonly Rational Zero = new Rational(0, 1);

			public Rational(int numerator, int denominator)
			{
				if (denominator <= 0)
					throw new ArgumentException("denominator must be positive");
				this.numerator = numerator;
				this.denominator = numerator == 0 ? 1 : denominator;
				Reduce();
			}

			public static explicit operator int(Rational rat)
			{
				return rat.numerator / rat.denominator;
			}

			public static Rational operator +(Rational rat1, Rational rat2)
			{
				if (rat1.numerator == 0) return rat2;
				if (rat2.numerator == 0) return rat1;

				if (rat1.denominator == rat2.denominator)
					return new Rational(rat1.numerator + rat2.numerator, rat1.denominator);
				else
					return new Rational(rat1.numerator * rat2.denominator + rat2.numerator * rat1.denominator, rat1.denominator * rat2.denominator);
			}

			public static Rational operator /(Rational rat1, Rational rat2)
			{
				return new Rational(rat1.numerator * rat2.denominator, rat1.denominator * rat2.numerator);
			}

			public static Rational operator /(Rational rat1, int scalar)
			{
				return new Rational(rat1.numerator, rat1.denominator * scalar);
			}

			public static Rational operator /(int scalar, Rational rat1)
			{
				return new Rational(rat1.denominator * scalar, rat1.numerator);
			}

			public static Rational operator *(Rational rat1, Rational rat2)
			{
				return new Rational(rat1.numerator * rat2.numerator, rat1.denominator * rat2.denominator);
			}

			public static Rational operator *(Rational rat1, int scalar)
			{
				return new Rational(rat1.numerator * scalar, rat1.denominator);
			}

			private void Reduce()
			{
				int ggt = GgT(numerator, denominator);
				if (ggt > 1)
				{
					numerator /= ggt;
					denominator /= ggt;
				}
			}

			private int GgT(int a, int b)
			{
				if (b == 0) return 0;
				int zahl1 = a;
				int zahl2 = b;
				while (zahl1 % zahl2 != 0)
				{
					int temp = zahl1 % zahl2;
					zahl1 = zahl2;
					zahl2 = temp;
				}
				return zahl2;
			}

			private int KgV(int zahl1, int zahl2)
			{
				return (zahl1 * zahl2) / GgT(zahl1, zahl2);
			}

			public bool IsZero()
			{
				return numerator == 0;
			}

			public override string ToString()
			{
				return numerator + "/" + denominator;
			}
		}
	}
}
