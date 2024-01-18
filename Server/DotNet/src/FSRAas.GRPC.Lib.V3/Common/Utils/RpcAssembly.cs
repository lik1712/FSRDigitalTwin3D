using System.Reflection;

namespace FSRAas.GRPC.Lib.V3.Common.Utils;

public class RpcAssembly {
    public static Assembly GetAssembly() {
        return typeof(RpcAssembly).Assembly;
    }
}