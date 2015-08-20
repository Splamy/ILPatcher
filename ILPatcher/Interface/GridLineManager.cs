using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ILPatcher.Interface
{
	class GridLineManager
	{
		private bool useGlobalLayout = false;

		private static int globalElementDistance = 5;
		private int localElementDistance = 20;
		public int ElementDistance
		{
			get { return useGlobalLayout ? globalElementDistance : localElementDistance; }
			set { if (useGlobalLayout) globalElementDistance = value; else localElementDistance = value; }
		}

		private Control parent;
		private List<LayoutElement> elementList;

		public GridLineManager(Control parent, bool globalLayout)
		{
			this.parent = parent;
			this.useGlobalLayout = globalLayout;
			elementList = new List<LayoutElement>();
			parent.Resize += parent_Resize;
		}

		void parent_Resize(object sender, EventArgs e)
		{
			GenerateAxisPositionValues(elementList, parent.ClientRectangle.Height);
			parent.SuspendLayout();
			foreach (var element in elementList)
			{
				element.Width = parent.ClientRectangle.Width; // pass width
				element.Left = 0; // pass left
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
				int location = 0;
				foreach (var element in listToSize)
				{
					element.curSize = element.minValue;
					element.curPosition = location;

					location += element.curSize;
				}
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

		private static Rational GetFractionSum(List<LayoutElement> fractList, int ofMaximum)
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
			var elem = new GridLine(this, arrType, minHeight);
			elem.prefValue = maxHeight;
			elementList.Add(elem);
			return elementList.Count - 1;
		}

		// Add elements

		public void AddElementFixed(int line, Control child, int width)
		{
			AddElementInternal(line, child, ArrangementType.Fixed, width, -1);
		}

		public void AddElementStretchable(int line, Control child, int minWidth, int maxWidth)
		{
			AddElementInternal(line, child, ArrangementType.PreferredAndShrink, minWidth, maxWidth);
		}

		public void AddElementFilling(int line, Control child, int minWidth)
		{
			AddElementInternal(line, child, ArrangementType.Fill, minWidth, -1);
		}

		private void AddElementInternal(int line, Control child, ArrangementType arrType, int minWidth, int maxWidth)
		{
			LayoutElement element = null;
			if (child == null)
			{
				element = new BlankElement(arrType, minWidth);
			}
			else
			{
				parent.Controls.Add(child);
				element = new GridElement(this, child, arrType, minWidth);
			}
			element.prefValue = maxWidth;
			elementList[line].AddElement(element);
		}

		// Change elements

		public void SwapControl(int line, int controlNum, Control newControl)
		{
			LayoutElement lElement = elementList?[line].GetElement(controlNum);
			if (lElement == null) throw new ArgumentException("The element doesn't exist");

			if (lElement is GridElement)
			{
				GridElement gElem = lElement as GridElement;
				gElem.control = newControl;
            }
			else if (lElement is BlankElement)
			{
				throw new NotSupportedException("Feature is not yet working");
			}
			else
				throw new InvalidOperationException($"Unknown element type in List: {lElement.GetType().Name}");
		}

		// Class structures

		private class GridLine : LayoutElement
		{
			public List<LayoutElement> elementList;

			public GridLine(GridLineManager manager, ArrangementType arrTyp, int minVal)
				: base(manager, arrTyp, minVal)
			{
				elementList = new List<LayoutElement>();
			}

			public override void MatchContent()
			{
				manager.GenerateAxisPositionValues(elementList, Width);
				foreach (var element in elementList)
				{
					element.Left = Left; // pass left
					element.Top = curPosition; // adjust top
					element.Width = Width; // pass width
					element.Height = curSize; // adjust height
					element.MatchContent();
				}
			}

			public override int AddElement(LayoutElement element)
			{
				elementList.Add(element);
				return elementList.Count - 1;
			}

			public void RemoveElement(LayoutElement element)
			{
				elementList.Remove(element);
			}

			public override LayoutElement GetElement(int controlNumber)
			{
				return elementList?[controlNumber];
			}
		}

		private class BlankElement : LayoutElement
		{
			public BlankElement(ArrangementType arrTyp, int minVal)
				: base(null, arrTyp, minVal)
			{ }

			public override void MatchContent() { }
		}

		private class GridElement : LayoutElement
		{
			public Control control;

			public GridElement(GridLineManager manager, Control control, ArrangementType arrTyp, int minVal)
				: base(manager, arrTyp, minVal)
			{
				this.control = control;
			}

			public override void MatchContent()
			{
				control.Left = curPosition; // adjust left
				control.Top = Top; // pass top
				control.Width = curSize; // adjust width
				control.Height = Height; // pass height
			}
		}

		private abstract class LayoutElement
		{
			public GridLineManager manager;
			public ArrangementType arrangementType;
			public int minValue;
			public int prefValue;

			public int curPosition;
			public int curSize;
			public Rational fractionOfMax;

			public int Left;
			public int Top;
			public int Width;
			public int Height;

			protected LayoutElement(GridLineManager manager, ArrangementType arrType, int minVal)
			{
				this.manager = manager;
				arrangementType = arrType;
				minValue = minVal;
			}

			public abstract void MatchContent();
			public virtual int AddElement(LayoutElement layoutElement) { throw new NotSupportedException(); }
			public virtual LayoutElement GetElement(int controlNumber) { throw new NotSupportedException(); }
		}

		private enum ArrangementType
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
					throw new ArgumentException(nameof(denominator) + "must be positive");
				this.numerator = numerator;
				this.denominator = numerator == 0 ? 1 : denominator;
				Reduce();
			}

			public static explicit operator int (Rational rat)
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
