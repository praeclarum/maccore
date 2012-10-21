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

	public class CGPDFOperatorTable : INativeObject, IDisposable {
		internal IntPtr handle;

		~CGPDFOperatorTable ()
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
		extern static void CGPDFOperatorTableRelease (IntPtr handle);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFOperatorTableRetain (IntPtr handle);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGPDFOperatorTableRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		/* invoked by marshallers */
		public CGPDFOperatorTable (IntPtr handle)
		{
			this.handle = handle;
			CGPDFOperatorTableRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGPDFOperatorTable (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGPDFOperatorTableRetain (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGPDFOperatorTableCreate ();
		
		public CGPDFOperatorTable ()
		{
			handle = CGPDFOperatorTableCreate ();
		}

		delegate void CGPDFOperatorCallbackNative (IntPtr /* CGPDFScannerRef */ scanner, IntPtr info);
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static int CGPDFOperatorTableSetCallback (IntPtr /* CGPDFOperatorTableRef */ table,
		                                                 string /* const char * */ name,
		                                                 CGPDFOperatorCallbackNative callback);

		public void SetCallback (string name, CGPDFOperatorCallback callback)
		{
			CGPDFOperatorTableSetCallback (handle, name, (scannerHandle, info) => {
				var scanner = new CGPDFScanner (scannerHandle);
				callback (scanner, info);
			});
		}
	}

	public delegate void CGPDFOperatorCallback (CGPDFScanner scanner, IntPtr info);
}
