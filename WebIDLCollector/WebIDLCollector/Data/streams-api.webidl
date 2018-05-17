[Constructor(StreamInit streamInitDict)]
interface ReadableStream
{
    readonly attribute boolean locked;

    Promise<void> cancel(optional any reason);
    ReadableStream getReader(optional ReaderMode mode);
    ReadableStream pipeThrough(TransformStream transform, optional any options);
    Promise<WritableStream> pipeTo(WritableStream dest, optional PipeToInit pipeToInitDict);
    Array tee();
};

dictionary StreamInit
{
    any underlyingSource;
    any strategy;
};

enum ReaderMode
{
    "byob"
};

dictionary PipeToInit
{
    boolean preventClose;
    boolean preventAbort;
    boolean preventCancel;
};

[Constructor(ReadableStream stream)]
interface ReadableStreamDefaultReader
{
    readonly attribute Promise<void> closed;
    Promise<void> cancel(optional any reason);
    Promise<any> read();
    void releaseLock();
};

[Constructor(ReadableStream stream)]
interface ReadableStreamBYOBReader : ReadableStreamDefaultReader
{
};

[Constructor]
interface ReadableStreamDefaultController
{
    readonly attribute long long? desiredSize;
    void close();
    any enqueue(any chunk);
    any error(Event event);
};

[Constructor]
interface ReadableByteStreamController : ReadableStreamDefaultController
{
    readonly attribute ReadableStreamBYOBReader byobRequest;
};

[Constructor(ReadableByteStreamController controller, ArrayBufferView view)]
interface ReadableStreamBYOBRequest
{
    readonly attribute ArrayBufferView view;

    ReadableByteStreamController respond(float bytesWritten);
    ReadableByteStreamController respondWithNewView(ArrayBufferView view);
};

[Constructor(StreamInit streamInitDict)]
interface WritableStream
{
    readonly attribute boolean locked;

    Promise<void> abort(optional any reason);
    WritableStreamDefaultWriter getWriter();
};

[Constructor(WritableStream stream)]
interface WritableStreamDefaultReader
{
    readonly attribute Promise<void> closed;
    readonly attribute long long? desiredSize;
    readonly attribute Promise<void> ready;

    Promise<void> abort(optional any reason);
    void close();
    void releaseLock();
    Promise<any> write(any chunk);
};

[Constructor]
interface WritableStreamDefaultController
{
    any error(Event event);
};

[Constructor(TransformStreamInit transformStreamInitDict)]
interface TransformStream
{
    readonly attribute any readable;
    readonly attribute any writable;
};

dictionary TransformStreamInit
{
    any transformer;
    any writableStrategy;
    any readableStrategy;
};

[Constructor]
interface TransformStreamDefaultController
{
    readonly attribute long long? desiredSize;

    any enqueue(any chunk);
    any error(Event event);
    void terminate();
};

[Constructor(QueuingStrategyInit queuingStrategyInitDict)]
interface ByteLengthQueuingStrategy
{
    unsigned long long size(any chunk);
};

dictionary QueuingStrategyInit
{
    unsigned long long highWaterMark;
};

[Constructor(QueuingStrategyInit queuingStrategyInitDict)]
interface CountQueuingStrategy
{
    unsigned long long size(any chunk);
};
