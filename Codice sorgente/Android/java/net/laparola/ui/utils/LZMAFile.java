package net.laparola.ui.utils;

import java.io.InputStream;
import java.io.OutputStream;

import net.laparola.ui.utils.lzma_java.LZMADecoder;

public class LZMAFile {
	/*
	public static void decomprimi (String inFileName, String outFileName) throws Exception {
		File inFile = new File(inFileName);
		File outFile = new File(outFileName);
		
		BufferedInputStream inStream  = new BufferedInputStream(new FileInputStream(inFile));
		BufferedOutputStream outStream = new BufferedOutputStream(new FileOutputStream(outFile));
		
		decomprimi(inStream, outStream);
	}
	 */
	
	public static void decomprimi(InputStream inStream, OutputStream outStream, LZMADecoder.ProgressRunnable progress) throws Exception {
		int propertiesSize = 5;
		byte[] properties = new byte[propertiesSize];
		if (inStream.read(properties, 0, propertiesSize) != propertiesSize)
			throw new Exception("input .lzma file is too short");
		LZMADecoder decoder = new LZMADecoder();
		if (!decoder.SetDecoderProperties(properties))
			throw new Exception("Incorrect stream properties");
		long outSize = 0;
		for (int i = 0; i < 8; i++)
		{
			int v = inStream.read();
			if (v < 0)
				throw new Exception("Can't read stream size");
			outSize |= ((long)v) << (8 * i);
		}
		if (!decoder.Code(inStream, outStream, outSize, progress))
			throw new Exception("Error in data stream");
		
		outStream.close();
		inStream.close();
	}
}
