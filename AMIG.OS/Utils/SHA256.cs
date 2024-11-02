using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMIG.OS.Utils
{

    // Implementierung des SHA256-Algorithmus
    internal class SHA256
    {
        private static readonly uint[] K =
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
            0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
            0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
            0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
            0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
            0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
            0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19b4c79b, 0x1e376c4f, 0x2748774c, 0x34b0bcb5,
            0x391c0cb3, 0x4ed8aa11, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
            0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        public static byte[] Hash(byte[] input)
        {
            // Padding und Initialisierung
            int originalByteLength = input.Length;
            int originalBitLength = originalByteLength * 8;

            // Padding hinzufügen
            Array.Resize(ref input, originalByteLength + 1);
            input[originalByteLength] = 0x80; // Append the '1' bit

            int totalLength = originalByteLength + 1;
            while ((totalLength * 8) % 512 != 448)
            {
                Array.Resize(ref input, totalLength + 1);
                input[totalLength] = 0; // Append '0' bits
                totalLength++;
            }

            // Füge die ursprüngliche Bitlänge als 64-Bit Wert hinzu
            Array.Resize(ref input, totalLength + 8);
            BitConverter.GetBytes((ulong)originalBitLength).CopyTo(input, totalLength);

            uint[] hash = new uint[8]
            {
                0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
                0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
            };

            for (int i = 0; i < input.Length / 64; i++)
            {
                uint[] w = new uint[64];

                for (int j = 0; j < 16; j++)
                {
                    w[j] = BitConverter.ToUInt32(input, (i * 64) + (j * 4));
                }

                for (int j = 16; j < 64; j++)
                {
                    w[j] = Sigma1(w[j - 2]) + w[j - 7] + Sigma0(w[j - 15]) + w[j - 16];
                }

                uint a = hash[0];
                uint b = hash[1];
                uint c = hash[2];
                uint d = hash[3];
                uint e = hash[4];
                uint f = hash[5];
                uint g = hash[6];
                uint h = hash[7];

                for (int j = 0; j < 64; j++)
                {
                    uint temp1 = h + Sigma1(e) + Ch(e, f, g) + K[j] + w[j];
                    uint temp2 = Sigma0(a) + Maj(a, b, c);

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                hash[0] += a;
                hash[1] += b;
                hash[2] += c;
                hash[3] += d;
                hash[4] += e;
                hash[5] += f;
                hash[6] += g;
                hash[7] += h;
            }

            // Konvertiere die Hash-Werte in ein Byte-Array
            byte[] hashValue = new byte[32];
            for (int i = 0; i < 8; i++)
            {
                BitConverter.GetBytes(hash[i]).CopyTo(hashValue, i * 4);
            }

            return hashValue;
        }

        // Hilfsfunktionen für SHA256
        private static uint Ch(uint x, uint y, uint z) => (x & y) ^ (~x & z);
        private static uint Maj(uint x, uint y, uint z) => (x & y) ^ (x & z) ^ (y & z);
        private static uint Sigma0(uint x) => (RotateRight(x, 7) ^ RotateRight(x, 18) ^ (x >> 3));
        private static uint Sigma1(uint x) => (RotateRight(x, 17) ^ RotateRight(x, 19) ^ (x >> 10));
        private static uint RotateRight(uint x, int n) => (x >> n) | (x << (32 - n));
    }
}


