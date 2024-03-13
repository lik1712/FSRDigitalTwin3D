using System.Reflection;

namespace FSR.Aas.GRPC.Lib.V3.Common.Utils;

public class RpcAssembly {
    public static Assembly GetAssembly() {
        return typeof(RpcAssembly).Assembly;
    }
}