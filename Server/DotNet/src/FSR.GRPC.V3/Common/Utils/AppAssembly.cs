using System.Reflection;

namespace FSR.GRPC.V3.Common.Utils;

public class AppAssembly {
    public static Assembly GetAssembly() {
        return typeof(AppAssembly).Assembly;
    }
}