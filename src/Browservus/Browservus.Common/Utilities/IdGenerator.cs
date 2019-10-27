﻿using System;
using System.Threading;

namespace StEn.Browservus.Common.Utilities
{
	/// <summary>
	/// Inspired by <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>,
	/// this class generates an efficient 20-bytes ID which is the concatenation of a <c>base36</c> encoded
	/// machine name and <c>base32</c> encoded <see cref="long"/> using the alphabet <c>0-9</c> and <c>A-V</c>.
	/// </summary>
	public sealed class IDGenerator
	{
		private const string Encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

		private static readonly ThreadLocal<char[]> CharBufferThreadLocal =
			new ThreadLocal<char[]>(() =>
			{
				var buffer = new char[20];
				buffer[0] = Prefix[0];
				buffer[1] = Prefix[1];
				buffer[2] = Prefix[2];
				buffer[3] = Prefix[3];
				buffer[4] = Prefix[4];
				buffer[5] = Prefix[5];
				buffer[6] = '-';
				return buffer;
			});

		private static readonly char[] Prefix = new char[6];
		private static long lastId = DateTime.UtcNow.Ticks;

		static IDGenerator() => PopulatePrefix();

		private IDGenerator()
		{
		}

		/// <summary>
		/// Gets a single instance of the <see cref="IDGenerator"/>.
		/// </summary>
		public static IDGenerator Instance { get; } = new IDGenerator();

		/// <summary>
		/// Gets an ID. e.g: <c>XOGLN1-0HLHI1F5INOFA</c>.
		/// </summary>
		public string Next => GenerateImpl(Interlocked.Increment(ref lastId));

		private static string GenerateImpl(long id)
		{
			var buffer = CharBufferThreadLocal.Value;

			buffer[7] = Encode32Chars[(int)(id >> 60) & 31];
			buffer[8] = Encode32Chars[(int)(id >> 55) & 31];
			buffer[9] = Encode32Chars[(int)(id >> 50) & 31];
			buffer[10] = Encode32Chars[(int)(id >> 45) & 31];
			buffer[11] = Encode32Chars[(int)(id >> 40) & 31];
			buffer[12] = Encode32Chars[(int)(id >> 35) & 31];
			buffer[13] = Encode32Chars[(int)(id >> 30) & 31];
			buffer[14] = Encode32Chars[(int)(id >> 25) & 31];
			buffer[15] = Encode32Chars[(int)(id >> 20) & 31];
			buffer[16] = Encode32Chars[(int)(id >> 15) & 31];
			buffer[17] = Encode32Chars[(int)(id >> 10) & 31];
			buffer[18] = Encode32Chars[(int)(id >> 5) & 31];
			buffer[19] = Encode32Chars[(int)id & 31];

			return new string(buffer, 0, buffer.Length);
		}

		private static void PopulatePrefix()
		{
			var machineHash = Math.Abs(Environment.MachineName.GetHashCode());
			var machineEncoded = Base36.Encode(machineHash);

			var i = Prefix.Length - 1;
			var j = 0;
			while (i >= 0)
			{
				if (j < machineEncoded.Length)
				{
					Prefix[i] = machineEncoded[j];
					j++;
				}
				else
				{
					Prefix[i] = '0';
				}

				i--;
			}
		}
	}
}
