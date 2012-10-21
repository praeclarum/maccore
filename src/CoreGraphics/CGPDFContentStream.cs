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

	public class CGPDFContentStream : INativeObject, IDisposable {
		internal IntPtr handle;

		~CGPDFContentStream ()
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
		extern static void CGPDFContentStreamRelease (IntPtr handle);
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContentStreamRetain (IntPtr handle);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGPDFContentStreamRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		/* invoked by marshallers */
		public CGPDFContentStream (IntPtr handle)
		{
			this.handle = handle;
			CGPDFContentStreamRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGPDFContentStream (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGPDFContentStreamRetain (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGPDFContentStreamCreateWithPage (IntPtr /* CGPDFPageRef */ page);
		
		public CGPDFContentStream (CGPDFPage page)
		{
			if (page == null)
				throw new ArgumentNullException ("page");
			handle = CGPDFContentStreamCreateWithPage (page.Handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static int CGPDFDocumentGetNumberOfPages (IntPtr handle);

		public int Pages {
			get {
				return CGPDFDocumentGetNumberOfPages (handle);
			}
		}
	}
}
