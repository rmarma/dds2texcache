/*
Конвертирует dds в texcache.
Версия: 0.0.1
Автор: Р.М.А.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace Dds2Texcache
{
	public class MainClass
	{
		public const string NAME = "dds2texcache";
		public const string DESCRIPTION = "Конвертер из DDS в TEXCACHE.";
		public const string VERSION = "0.0.1";
		public const string AUTHOR = "Р.М.А.";
		public const string YEAR = "2015";
		public const string HELP = "Перетащите файл DDS поверх файла программы.";
		public const string CONVERTED = "Преобразовано: {0} из {1}.";
		public const string EXIT = "Для выхода нажмите \"ВВОД\".";
		
		public const string EXT_DDS = "dds";
		public const string EXT_TEXCACHE = "texcache";
		
		public static void Main(string[] args)
		{
			if (args == null || args.Length <= 0)
			{
				Console.WriteLine(NAME + " (" + VERSION + ")\t" + AUTHOR + ", " + YEAR);
				Console.WriteLine("------------");
				Console.WriteLine(DESCRIPTION);
				Console.WriteLine("------------");
				Console.WriteLine(HELP);
			}
			else
			{
				int count = 0;
				foreach (string p in args)
				{
					try
					{
						string p2 = p;
						int ddsIndex = p2.LastIndexOf(EXT_DDS);
						if (ddsIndex != -1)
						{
							p2 = p2.Remove(ddsIndex);
						}
						p2 += EXT_TEXCACHE;
						new FileTexcache().FromDDS(new FileDDS(p)).Save(p2);
						++count;
						Console.Write('.');
					}
					catch (Exception e)
					{
						Console.WriteLine();
						Console.WriteLine(e.Message);
					}
				}
				Console.WriteLine();
				Console.WriteLine(CONVERTED, count, args.Length);
			}
			Console.WriteLine("\n" + EXIT);
			Console.ReadKey();
		}
	}
	
	/// <summary>
	/// Класс для работы с .dds.
	/// </summary>
	public class FileDDS
	{
		/// <summary>
		/// Размер заголовка без метки.
		/// </summary>
		public const uint HDR_SIZE = 124;
		
		public const string DXT1 = "DXT1";
		public const string DXT2 = "DXT2";
		public const string DXT3 = "DXT3";
		public const string DXT4 = "DXT4";
		public const string DXT5 = "DXT5";
		public const string RGB8 = "RGB8";
		public const string ARGB = "ARGB";
		
		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		public const uint DDSD_CAPS = 0x1;
		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		public const uint DDSD_HEIGHT = 0x2;
		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		public const uint DDSD_WIDTH = 0x4;
		/// <summary>
		/// Required when pitch is provided for an uncompressed texture.
		/// </summary>
		public const uint DDSD_PITCH = 0x8;
		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		public const uint DDSD_PIXELFORMAT = 0x1000;
		/// <summary>
		/// Required in a mipmapped texture.
		/// </summary>
		public const uint DDSD_MIPMAPCOUNT = 0x20000;
		/// <summary>
		/// Required when pitch is provided for a compressed texture.
		/// </summary>
		public const uint DDSD_LINEARSIZE = 0x80000;
		/// <summary>
		/// Required in a depth texture.
		/// </summary>
		public const uint DDSD_DEPTH = 0x800000;
		
		/// <summary>
		/// Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
		/// </summary>
		public const uint DDSCAPS_COMPLEX = 0x8;
		/// <summary>
		/// Optional; should be used for a mipmap.
		/// </summary>
		public const uint DDSCAPS_MIPMAP = 0x400000;
		/// <summary>
		/// Required
		/// </summary>
		public const uint DDSCAPS_TEXTURE = 0x1000;
		
		/// <summary>
		/// Required for a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP = 0x200;
		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP_POSITIVEX = 0x400;
		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800;
		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000;
		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000;
		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000;
		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		public const uint DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000;
		/// <summary>
		/// Required for a volume texture.
		/// </summary>
		public const uint DDSCAPS2_VOLUME = 0x200000;
		
		/// <summary>
		/// Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
		/// </summary>
		public const uint DDPF_ALPHAPIXELS = 0x1;
		/// <summary>
		/// Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
		/// </summary>
		public const uint DDPF_ALPHA = 0x2;
		/// <summary>
		/// Texture contains compressed RGB data; dwFourCC contains valid data.
		/// </summary>
		public const uint DDPF_FOURCC = 0x4;
		/// <summary>
		/// Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
		/// </summary>
		public const uint DDPF_RGB = 0x40;
		/// <summary>
		/// Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
		/// </summary>
		public const uint DDPF_YUV = 0x200;
		/// <summary>
		/// Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
		/// </summary>
		public const uint DDPF_LUMINANCE = 0x20000;
		
		
		public struct HeaderStruct
		{
			/// <summary>
			/// Size of structure. This member must be set to 124.
			/// </summary>
			public uint dwSize;
			/// <summary>
			/// Flags to indicate which members contain valid data.
			/// </summary>
			public uint dwFlags;
			/// <summary>
			/// Surface height (in pixels).
			/// </summary>
			public uint dwHeight;
			/// <summary>
			/// Surface width (in pixels).
			/// </summary>
			public uint dwWidth;
			/// <summary>
			/// The pitch or number of bytes per scan line in an uncompressed texture; the total number of bytes in the top level texture for a compressed texture. For information about how to compute the pitch, see the DDS File Layout section of the Programming Guide for DDS.
			/// </summary>
			public uint dwPitchOrLinearSize;
			/// <summary>
			/// Depth of a volume texture (in pixels), otherwise unused.
			/// </summary>
			public uint dwDepth;
			/// <summary>
			/// Number of mipmap levels, otherwise unused.
			/// </summary>
			public uint dwMipMapCount;
			/// <summary>
			/// Unused. Length = 11.
			/// </summary>
			public uint[] dwReserved1;
			/// <summary>
			/// The pixel format (see DDS_PIXELFORMAT).
			/// </summary>
			public PixelFormatStruct pixelFormat;
			/// <summary>
			/// Specifies the complexity of the surfaces stored.
			/// When you write .dds files, you should set the DDSCAPS_TEXTURE flag, and for multiple surfaces you should also set the DDSCAPS_COMPLEX flag. However, when you read a .dds file, you should not rely on the DDSCAPS_TEXTURE and DDSCAPS_COMPLEX flags being set because some writers of such a file might not set these flags.
			/// The DDS_SURFACE_FLAGS_MIPMAP flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS_COMPLEX and DDSCAPS_MIPMAP flags.
			/// The DDS_SURFACE_FLAGS_TEXTURE flag, which is defined in Dds.h, is equal to the DDSCAPS_TEXTURE flag.
			/// The DDS_SURFACE_FLAGS_CUBEMAP flag, which is defined in Dds.h, is equal to the DDSCAPS_COMPLEX flag.
			/// </summary>
			public uint dwCaps;
			/// <summary>
			/// Additional detail about the surfaces stored.
			/// The DDS_CUBEMAP_POSITIVEX flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS2_CUBEMAP and DDSCAPS2_CUBEMAP_POSITIVEX flags.
			/// The DDS_CUBEMAP_NEGATIVEX flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS2_CUBEMAP and DDSCAPS2_CUBEMAP_NEGATIVEX flags.
			/// The DDS_CUBEMAP_POSITIVEY flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS2_CUBEMAP and DDSCAPS2_CUBEMAP_POSITIVEY flags.
			/// The DDS_CUBEMAP_NEGATIVEY flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS2_CUBEMAP and DDSCAPS2_CUBEMAP_NEGATIVEY flags.
			/// The DDS_CUBEMAP_POSITIVEZ flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS2_CUBEMAP and DDSCAPS2_CUBEMAP_POSITIVEZ flags.
			/// The DDS_CUBEMAP_NEGATIVEZ flag, which is defined in Dds.h, is a bitwise-OR combination of the DDSCAPS2_CUBEMAP and DDSCAPS2_CUBEMAP_NEGATIVEZ flags.
			/// The DDS_CUBEMAP_ALLFACES flag, which is defined in Dds.h, is a bitwise-OR combination of the DDS_CUBEMAP_POSITIVEX, DDS_CUBEMAP_NEGATIVEX, DDS_CUBEMAP_POSITIVEY, DDS_CUBEMAP_NEGATIVEY, DDS_CUBEMAP_POSITIVEZ, and DDSCAPS2_CUBEMAP_NEGATIVEZ flags.
			/// The DDS_FLAGS_VOLUME flag, which is defined in Dds.h, is equal to the DDSCAPS2_VOLUME flag.
			/// Although Direct3D 9 supports partial cube-maps, Direct3D 10, 10.1, and 11 require that you define all six cube-map faces (that is, you must set DDS_CUBEMAP_ALLFACES).
			/// </summary>
			public uint dwCaps2;
			/// <summary>
			/// Unused.
			/// </summary>
			public uint dwCaps3;
			/// <summary>
			/// Unused.
			/// </summary>
			public uint dwCaps4;
			/// <summary>
			/// Unused.
			/// </summary>
			public uint dwReserved2;
		}
		
		
		public struct PixelFormatStruct
		{
			/// <summary>
			/// Structure size; set to 32 (bytes).
			/// </summary>
			public uint dwSize;
			/// <summary>
			/// Values which indicate what type of data is in the surface.
			/// </summary>
			public uint dwFlags;
			/// <summary>
			/// Four-character codes for specifying compressed or custom formats. Possible values include: DXT1, DXT2, DXT3, DXT4, or DXT5. A FourCC of DX10 indicates the prescense of the DDS_HEADER_DXT10 extended header, and the dxgiFormat member of that structure indicates the true format. When using a four-character code, dwFlags must include DDPF_FOURCC.
			/// </summary>
			public string dwFourCC;
			/// <summary>
			/// Number of bits in an RGB (possibly including alpha) format. Valid when dwFlags includes DDPF_RGB, DDPF_LUMINANCE, or DDPF_YUV.
			/// </summary>
			public uint dwRGBBitCount;
			/// <summary>
			/// Red (or lumiannce or Y) mask for reading color data. For instance, given the A8R8G8B8 format, the red mask would be 0x00ff0000.
			/// </summary>
			public uint dwRBitMask;
			/// <summary>
			/// Green (or U) mask for reading color data. For instance, given the A8R8G8B8 format, the green mask would be 0x0000ff00.
			/// </summary>
			public uint dwGBitMask;
			/// <summary>
			/// Blue (or V) mask for reading color data. For instance, given the A8R8G8B8 format, the blue mask would be 0x000000ff.
			/// </summary>
			public uint dwBBitMask;
			/// <summary>
			/// Alpha mask for reading alpha data. dwFlags must include DDPF_ALPHAPIXELS or DDPF_ALPHA. For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
			/// </summary>
			public uint dwABitMask;
		}
		
		/// <summary>
		/// Метка файла перед заголовком ("DDS").
		/// </summary>
		public string mark;
		/// <summary>
		/// Заголовок.
		/// </summary>
		public HeaderStruct header;
		/// <summary>
		/// Массив пикселей.
		/// </summary>
		public byte[] pixelData;
		
		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="path">Путь к файлу .dds.</param>
		public FileDDS(string path)
		{
			Initialize(path);
		}
		
		/// <summary>
		/// Инициализация.
		/// </summary>
		/// <param name="path">Путь к файлу .dds.</param>
		private void Initialize(string path)
		{
			byte[] buffer = null;
			using (FileStream fs = File.OpenRead(path))
			{
				buffer = new byte[fs.Length];
				fs.Read(buffer, 0, buffer.Length);
			}
			SetData(buffer);
		}
		
		/// <summary>
		/// Разбор массива байтов и установка параметров.
		/// </summary>
		/// <param name="data">Прочитанный массив байтов файла .dds.</param>
		public void SetData(byte[] data)
		{
			if (data != null && data.Length > HDR_SIZE)
			{
				int offset = 4;
				HeaderStruct hdr = new HeaderStruct();
				// определение метки перед заголовком
				if (BitConverter.ToUInt32(data, 0) != HDR_SIZE)
				{
					mark = System.Text.Encoding.ASCII.GetString(data, 0, 4);
					hdr.dwSize = BitConverter.ToUInt32(data, 4);
					if (hdr.dwSize != HDR_SIZE)
					{
						throw new Exception("hdr.dwSize != HDR_SIZE");
					}
				}
				else if (BitConverter.ToUInt32(data, 4) != HDR_SIZE)
				{
					offset = 0;
					mark = String.Empty;
					hdr.dwSize = BitConverter.ToUInt32(data, 0);
				}
				else
				{
					mark = System.Text.Encoding.ASCII.GetString(data, 0, 4);
					hdr.dwSize = BitConverter.ToUInt32(data, 4);
				}
				hdr.dwFlags = BitConverter.ToUInt32(data, offset + 4);
				hdr.dwHeight = BitConverter.ToUInt32(data, offset + 8);
				hdr.dwWidth = BitConverter.ToUInt32(data, offset + 12);
				hdr.dwPitchOrLinearSize = BitConverter.ToUInt32(data, offset + 16);
				hdr.dwDepth = BitConverter.ToUInt32(data, offset + 20);
				hdr.dwMipMapCount = BitConverter.ToUInt32(data, offset + 24);
				hdr.dwReserved1 = new uint[11];
				hdr.pixelFormat = new PixelFormatStruct();
				hdr.pixelFormat.dwSize = BitConverter.ToUInt32(data, offset + 72);
				hdr.pixelFormat.dwFlags = BitConverter.ToUInt32(data, offset + 76);
				if ((hdr.pixelFormat.dwFlags & DDPF_FOURCC) == DDPF_FOURCC)
				{
					hdr.pixelFormat.dwFourCC = System.Text.Encoding.ASCII.GetString(data, offset + 80, 4);
				}
				else if ((hdr.pixelFormat.dwFlags & DDPF_RGB) == DDPF_RGB)
				{
					if ((hdr.pixelFormat.dwFlags & DDPF_ALPHAPIXELS) == DDPF_ALPHAPIXELS)
					{
						hdr.pixelFormat.dwFourCC = ARGB;
					}
					else
					{
						hdr.pixelFormat.dwFourCC = RGB8;
					}
				}
				else
				{
					hdr.pixelFormat.dwFourCC = String.Empty;
				}
				hdr.pixelFormat.dwRGBBitCount = BitConverter.ToUInt32(data, offset + 84);
				hdr.pixelFormat.dwRBitMask = BitConverter.ToUInt32(data, offset + 88);
				hdr.pixelFormat.dwGBitMask = BitConverter.ToUInt32(data, offset + 92);
				hdr.pixelFormat.dwBBitMask = BitConverter.ToUInt32(data, offset + 96);
				hdr.pixelFormat.dwABitMask = BitConverter.ToUInt32(data, offset + 100);
				hdr.dwCaps = BitConverter.ToUInt32(data, offset + 104);
				hdr.dwCaps2 = BitConverter.ToUInt32(data, offset + 108);
				hdr.dwCaps3 = BitConverter.ToUInt32(data, offset + 112);
				hdr.dwCaps4 = BitConverter.ToUInt32(data, offset + 116);
				hdr.dwReserved2 = BitConverter.ToUInt32(data, offset + 120);
				byte[] pxData = new byte[data.Length - (HDR_SIZE + offset)];
				System.Array.Copy(data, HDR_SIZE + offset, pxData, 0, pxData.Length);
				pixelData = pxData;
				header = hdr;
			}
			else
			{
				throw new Exception("data == null || data.Length <= HDR_SIZE");
			}
		}
	}
	
	/// <summary>
	/// Класс для работы с .texcache.
	/// </summary>
	public class FileTexcache
	{
		public const string HDR_STRING_10100 = "Gamebryo File Format, Version 10.1.0.0\n";
		public const string HDR_STRING_10200 = "Gamebryo File Format, Version 10.2.0.0\n";
		
		public const string NI_PIXEL_DATA = "NiPixelData";
		
		public const int VER_10100 = 0x0A010000;
		public const int VER_10200 = 0x0A020000;
		
		public const int USER_VER = 0x00;
		
		public const int PX_FMT_RGB8 = 0;
		public const int PX_FMT_RGBA8 = 1;
		public const int PX_FMT_DXT1 = 4;
		public const int PX_FMT_DXT5 = 5;
		
		public const uint MIPMAP_LAST_DELTA = 16;
		
		public struct HeaderStruct
		{
			/// <summary>
			/// A variable length string that ends with a newline character (0x0A). The string starts as follows depending on the version:
			/// Version Version >= 10.1.0.0: 'Gamebryo File Format'
			/// </summary>
			public string headerString;
			/// <summary>
			/// A 32-bit integer that stores the version in hexadecimal format with each byte representing a number in the version string.
			/// Some widely-used versions and their hex representation:
			/// 4.0.0.2: 0x04000002
			/// 4.1.0.12: 0x0401000C
			/// 4.2.0.2: 0x04020002
			/// 4.2.1.0: 0x04020100
			/// 4.2.2.0: 0x04020200
			/// 10.0.1.0: 0x0A000100
			/// 10.1.0.0: 0x0A010000
			/// 10.2.0.0: 0x0A020000
			/// 20.0.0.4: 0x14000004
			/// 20.0.0.5: 0x14000005
			/// </summary>
			public uint version;
			public uint userVersion;
			public uint numBlocks;
			public short numBlockTypes;
			public BlockTypeStruct[] blockTypes;
			public uint unknownInt;
			public uint blockSeparator;
		}
		
		public struct BlockTypeStruct
		{
			public uint length;
			public string name;
			public short index;
		}
		
		public struct PixelDataStruct
		{
			public uint pixelFormat;
			public uint redMask;
			public uint greenMask;
			public uint blueMask;
			public uint alphaMask;
			public byte bitsPerPixel;
			public byte[] unknown3Bytes;
			public byte[] unknown8Bytes;
			public uint unknownInt;
			public int palette;
			public uint numMipmaps;
			public uint bytesPerPixel;
			public MipmapStruct[] mipmaps;
			public uint numPixels;
			public byte[] pixelData;
		}
		
		public struct MipmapStruct
		{
			public uint width;
			public uint height;
			public uint offset;
		}
		
		public struct FooterStruct
		{
			public uint numRoots;
			public uint[] roots;
		}
		
		public HeaderStruct header;
		public PixelDataStruct pixelData;
		public FooterStruct footer;
		
		public FileTexcache() { }
		
		/// <summary>
		/// Создаёт .texcache файл из .dds файла.
		/// </summary>
		/// <param name="dds">Файл .dds.</param>
		public FileTexcache FromDDS(FileDDS dds)
		{
			FileTexcache texcache = new FileTexcache();
			
			HeaderStruct hdr = new HeaderStruct();
			hdr.headerString = HDR_STRING_10100;
			hdr.version = VER_10100;
			hdr.userVersion = USER_VER;
			hdr.numBlocks = 1;
			hdr.numBlockTypes = (short) 1;
			hdr.blockTypes = new BlockTypeStruct[1];
			hdr.blockTypes[0].name = NI_PIXEL_DATA;
			hdr.blockTypes[0].length = (uint) NI_PIXEL_DATA.Length;
			hdr.blockTypes[0].index = (short) 0;
			hdr.unknownInt = 0;
			hdr.blockSeparator = 0;
			
			PixelDataStruct pxData = new PixelDataStruct();
			// TODO: остальные фаорматы
			switch (dds.header.pixelFormat.dwFourCC)
			{
				case FileDDS.RGB8:
					{
						pxData.pixelFormat = PX_FMT_RGB8;
					}
					break;
				case FileDDS.ARGB:
					{
						pxData.pixelFormat = PX_FMT_RGBA8;
					}
					break;
				case FileDDS.DXT1:
					{
						pxData.pixelFormat = PX_FMT_DXT1;
					}
					break;
				case FileDDS.DXT5:
					{
						pxData.pixelFormat = PX_FMT_DXT5;
					}
					break;
				default:
					{
						pxData.pixelFormat = PX_FMT_DXT5;
					}
					break;
			}
			pxData.redMask = dds.header.pixelFormat.dwRBitMask;
			pxData.greenMask = dds.header.pixelFormat.dwGBitMask;
			pxData.blueMask = dds.header.pixelFormat.dwBBitMask;
			pxData.alphaMask = dds.header.pixelFormat.dwABitMask;
			pxData.bitsPerPixel = 0;
			pxData.unknown3Bytes = new byte[3];
			pxData.unknown8Bytes = new byte[8];
			pxData.unknown8Bytes[0] = (byte) (pxData.pixelFormat == PX_FMT_DXT5 ? 5 : 4);
			pxData.unknownInt = 0;
			pxData.palette = -1;
			pxData.numMipmaps = dds.header.dwMipMapCount;
			if (pxData.numMipmaps < 1) pxData.numMipmaps = 1;
			pxData.mipmaps = new MipmapStruct[pxData.numMipmaps];
			uint height = dds.header.dwHeight;
			uint width = dds.header.dwWidth;
			uint offset = 0;
			uint offsetDelta = 0;
			float offsetCoef = 1f;
			if (pxData.pixelFormat == PX_FMT_DXT1 || pxData.pixelFormat == PX_FMT_RGB8)
			{
				offsetCoef = 0.5f;
			}
			MipmapStruct mm;
			for (int i = 0; i < pxData.mipmaps.Length; ++i)
			{
				mm = new MipmapStruct();
				mm.height = height;
				mm.width = width;
				mm.offset = offset;
				pxData.mipmaps[i] = mm;
				
				offsetDelta = (uint) (height * width * offsetCoef);
				height /= 2;
				width /= 2;
				offsetDelta = (height == 1 || width == 1) ? (uint) (MIPMAP_LAST_DELTA * offsetCoef) : offsetDelta;
				offset += offsetDelta;
				if (height < 1) height = 1;
				if (width < 1) width = 1;
			}
			pxData.numPixels = (uint) dds.pixelData.Length;
			pxData.pixelData = dds.pixelData;
			
			FooterStruct ftr = new FooterStruct();
			ftr.numRoots = 1;
			ftr.roots = new uint[1];
			
			texcache.header = hdr;
			texcache.pixelData = pxData;
			texcache.footer = ftr;
			return texcache;
		}
		
		/// <summary>
		/// Сохраняет файл на диск.
		/// </summary>
		/// <param name="path">Путь куда надо сохранить файл.</param>
		public void Save(string path)
		{
			byte[] bytes = GetBytes();
			using (FileStream fs = File.OpenWrite(path))
			{
				fs.Write(bytes, 0, bytes.Length);
			}
		}
		
		/// <summary>
		/// Возвращает массив байт файла .texcache.
		/// </summary>
		/// <returns>Массив байт файла .texcache.</returns>
		public byte[] GetBytes()
		{
			List<byte> byteList = new List<byte>(pixelData.pixelData.Length * 2);
			byteList.AddRange(System.Text.Encoding.ASCII.GetBytes(header.headerString));
			byteList.AddRange(BitConverter.GetBytes(header.version));
			byteList.AddRange(BitConverter.GetBytes(header.userVersion));
			byteList.AddRange(BitConverter.GetBytes(header.numBlocks));
			byteList.AddRange(BitConverter.GetBytes(header.numBlockTypes));
			for (int i = 0; i < header.blockTypes.Length; ++i)
			{
				byteList.AddRange(BitConverter.GetBytes(header.blockTypes[i].length));
				byteList.AddRange(System.Text.Encoding.ASCII.GetBytes(header.blockTypes[i].name));
				byteList.AddRange(BitConverter.GetBytes(header.blockTypes[i].index));
			}
			byteList.AddRange(BitConverter.GetBytes(header.unknownInt));
			byteList.AddRange(BitConverter.GetBytes(header.blockSeparator));
			byteList.AddRange(BitConverter.GetBytes(pixelData.pixelFormat));
			byteList.AddRange(BitConverter.GetBytes(pixelData.redMask));
			byteList.AddRange(BitConverter.GetBytes(pixelData.greenMask));
			byteList.AddRange(BitConverter.GetBytes(pixelData.blueMask));
			byteList.AddRange(BitConverter.GetBytes(pixelData.alphaMask));
			byteList.Add(pixelData.bitsPerPixel);
			byteList.AddRange(pixelData.unknown3Bytes);
			byteList.AddRange(pixelData.unknown8Bytes);
			byteList.AddRange(BitConverter.GetBytes(pixelData.unknownInt));
			byteList.AddRange(BitConverter.GetBytes(pixelData.palette));
			byteList.AddRange(BitConverter.GetBytes(pixelData.numMipmaps));
			byteList.AddRange(BitConverter.GetBytes(pixelData.bytesPerPixel));
			for (int i = 0; i < pixelData.mipmaps.Length; ++i)
			{
				byteList.AddRange(BitConverter.GetBytes(pixelData.mipmaps[i].width));
				byteList.AddRange(BitConverter.GetBytes(pixelData.mipmaps[i].height));
				byteList.AddRange(BitConverter.GetBytes(pixelData.mipmaps[i].offset));
			}
			byteList.AddRange(BitConverter.GetBytes(pixelData.numPixels));
			byteList.AddRange(pixelData.pixelData);
			byteList.AddRange(BitConverter.GetBytes(footer.numRoots));
			for (int i = 0; i < footer.roots.Length; ++i)
			{
				byteList.AddRange(BitConverter.GetBytes(footer.roots[i]));
			}
			return byteList.ToArray();
		}
	}
}