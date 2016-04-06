[NoInterfaceObject, Exposed=(Window,Worker)]
interface NavigatorCPU {
  readonly attribute unsigned long hardwareConcurrency;
};

Navigator implements NavigatorCPU;
WorkerNavigator implements NavigatorCPU;