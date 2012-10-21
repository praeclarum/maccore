// 
// CGPDFDocument.cs: Implements the managed CGPDFDocument
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace MonoTouch.CoreGraphics {

	public class CGPDFScanner : INativeObject, IDisposable {
		internal IntPtr handle;

		~CGPDFScanner ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFScannerRelease (IntPtr handle);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFScannerRetain (IntPtr handle);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGPDFScannerRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		/* invoked by marshallers */
		public CGPDFScanner (IntPtr handle)
		{
			this.handle = handle;
			CGPDFScannerRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGPDFScanner (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGPDFScannerRetain (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGPDFScannerCreate (IntPtr /* CGPDFContentStreamRef */ cs, 
		                                         IntPtr /* CGPDFOperatorTableRef */ table, 
		                                         IntPtr info);
		
		public CGPDFScanner (CGPDFContentStream cs, CGPDFOperatorTable table, IntPtr info)
		{
			if (cs == null)
				throw new ArgumentNullException ("cs");
			if (table == null)
				throw new ArgumentNullException ("table");
			handle = CGPDFScannerCreate (cs.Handle, table.Handle, info);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerScan (IntPtr handle);

		public bool Scan ()
		{
			return CGPDFScannerScan (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopString (IntPtr /* CGPDFScannerRef */ scanner,
		                                          IntPtr /* CGPDFStringRef * */ value);
		
		public string PopString ()
		{
			string result = null;
			var pvalue = Marshal.AllocHGlobal (IntPtr.Size);
			var r = CGPDFScannerPopString (handle, pvalue);
			if (r) {
				var stringHandle = IntPtr.Zero;
				unsafe {
					stringHandle = *(IntPtr*)pvalue;
				}
				result = PdfStringCopyTextString (stringHandle);
			}
			Marshal.FreeHGlobal (pvalue);
			return result;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr /*CFStringRef*/ CGPDFStringCopyTextString (IntPtr /*CGPDFStringRef*/ pdfStr);
		
		static public string PdfStringCopyTextString (IntPtr pdfString)
		{
			if (pdfString == IntPtr.Zero)
				return null;			
			using (var cfs = new CFString (CGPDFStringCopyTextString (pdfString))) {
				return cfs.ToString ();
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopInteger (IntPtr /* CGPDFScannerRef */ scanner,
		                                           IntPtr /* CGPDFInteger * */ value);
		
		public int PopInteger ()
		{
			int result = 0;
			var pvalue = Marshal.AllocHGlobal (IntPtr.Size);
			var r = CGPDFScannerPopInteger (handle, pvalue);
			if (r) {
				unsafe {
					result = *(int*)pvalue;
				}
			}
			Marshal.FreeHGlobal (pvalue);
			return result;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopNumber (IntPtr /* CGPDFScannerRef */ scanner,
		                                          IntPtr /* CGPDFReal * */ value);
		
		public float PopNumber ()
		{
			float result = 0.0f;
			var pvalue = Marshal.AllocHGlobal (IntPtr.Size);
			var r = CGPDFScannerPopNumber (handle, pvalue);
			if (r) {
				unsafe {
					result = *(float*)pvalue;
				}
			}
			Marshal.FreeHGlobal (pvalue);
			return result;
		}		
	}
}
