using System;
using System.Collections.Generic;

namespace VoxelDare
{
	public interface ISerialize
	{
		void Read(List<byte> data);
		void Write(List<byte> data);
	}
}

