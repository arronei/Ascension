﻿partial interface Window
{
    attribute EventHandler ondeviceorientation;
};

[Constructor(DOMString type, optional DeviceOrientationEventInit eventInitDict)]
interface DeviceOrientationEvent : Event
{
    readonly attribute double? alpha;
    readonly attribute double? beta;
    readonly attribute double? gamma;
    readonly attribute boolean absolute;
};

dictionary DeviceOrientationEventInit : EventInit
{
    double? alpha = null;
    double? beta = null;
    double? gamma = null;
    boolean absolute = false;
};

partial interface Window
{
      attribute EventHandler ondevicemotion;
};

[NoInterfaceObject]
interface DeviceAcceleration
{
    readonly attribute double? x;
    readonly attribute double? y;
    readonly attribute double? z;
};

[NoInterfaceObject]
interface DeviceRotationRate
{
    readonly attribute double? alpha;
    readonly attribute double? beta;
    readonly attribute double? gamma;
};

[Constructor(DOMString type, optional DeviceMotionEventInit eventInitDict)]
interface DeviceMotionEvent : Event
{
    readonly attribute DeviceAcceleration? acceleration;
    readonly attribute DeviceAcceleration? accelerationIncludingGravity;
    readonly attribute DeviceRotationRate? rotationRate;
    readonly attribute double? interval;
};

dictionary DeviceAccelerationInit
{
    double? x = null;
    double? y = null;
    double? z = null;
};

dictionary DeviceRotationRateInit
{
    double? alpha = null;
    double? beta = null;
    double? gamma = null;
};

dictionary DeviceMotionEventInit : EventInit
{
    DeviceAccelerationInit? acceleration;
    DeviceAccelerationInit? accelerationIncludingGravity;
    DeviceRotationRateInit? rotationRate;
    double? interval = null;
};
