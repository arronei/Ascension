[Constructor, Constructor(unsigned long len), Constructor(any... args)]
interface Array {
    static Array from(any items, optional object mapfn, optional any thisArg);
    static boolean isArray(any arg);
    static Array of(any... items);
    Array concat(any... arguments);
    object copyWithin(long target, optional long start, optional long end);
    object entries();
    boolean every(object callbackfn, optional any thisArg);
    object fill(object value, optional long start, optional long end);
    Array filter(object callbackfn, optional any thisArg);
    void find(object predicate, optional any thisArg);
    long findIndex(object predicate, optional any thisArg);
    void forEach(object callbackfn, optional any thisArg);
    long indexOf(any searchElement, optional long fromIndex);
    DOMString join(DOMString separator);
    object keys();
    long lastIndexOf(any searchElement, optional long fromIndex);
    Array map(object callbackfn, optional any thisArg);
    object pop();
    unsigned long push(any... items);
    object reduce(object callbackfn, optional any initialValue);
    object reduceRight(object callbackfn, optional any initialValue);
    object reverse();
    any shift();
    Array slice(long start, long end);
    boolean some(object callbackfn, optional any thisArg);
    void sort(object comparefn);
    Array splice(unsigned long start, unsigned long deleteCount, any... items);
    DOMString toLocaleString(optional locales, optional options);
    DOMString toString();
    unsigned long unshift(any... items);
    object values();
    attribute unsigned long length;
}

[Constructor(unsigned long length)]
interface ArrayBuffer {
    static boolean isView(any arg);
    attribute unsigned long byteLength;
    ArrayBuffer slice(unsigned long start, unsigned long end);
}

[Constructor(optional boolean val)]
interface Boolean {
    DOMString toString();
    boolean valueOf();
}

[Constructor(object buffer, optional long byteOffset, optional unsigned long byteLength)]
interface DataView {
    attribute object buffer;
    attribute unsigned long byteLength;
    attribute unsigned long byteOffset;
    attribute DataView contructor;
    float getFloat32(float byteOffset, optional boolean littleEndian);
    double getFloat64(double byteOffset, optional boolean littleEndian);
    byte getInt8(byte byteOffset);
    short getInt16(short byteOffset, optional boolean littleEndian);
    long getInt32(long byteOffset, optional boolean littleEndian);
    octet getUint8(octet byteOffset);
    unsigned short getUint16(unsigned short byteOffset, optional boolean littleEndian);
    unsigned long getUint32(unsigned long byteOffset, optional boolean littleEndian);
    void setFloat32(float byteOffset, float value, optional boolean littleEndian);
    void setFloat64(double byteOffset, double value, optional boolean littleEndian);
    void setInt8(byte byteOffset, byte value, optional boolean littleEndian);
    void setInt16(short byteOffset, short value, optional boolean littleEndian);
    void setInt32(long byteOffset, long value, optional boolean littleEndian);
    void setUint8(octet byteOffset, octet value, optional boolean littleEndian);
    void setUint16(unsigned short byteOffset, unsigned short value, optional boolean littleEndian);
    void setUint32(unsigned long byteOffset, unsigned long value, optional boolean littleEndian);
}

[Constructor, Constructor(object value), Constructor(long year, long month, optional long date, optional long hours, optional long minutes, optional long seconds, optional long ms)]
interface Date {
    static object now();
    static object parse(DOMString string);
    static unsigned long UTC(long year, long month, optional long date, optional long hours, optional long minutes, optional long seconds, optional long ms);
    attribute Date contructor;
    octet getDate();
    octet getDay();
    unsigned long getFullYear();
    octet getHours();
    unsigned long getMilliseconds();
    unsigned short getMinutes();
    octet getMonth();
    unsigned long getSeconds();
    object getTime();
    octet getTimezoneOffset();
    octet getUTCDate();
    octet getUTCDay();
    unsigned long getUTCFullYear();
    octet getUTCHours();
    unsigned long getUTCMilliseconds();
    unsigned short getUTCMinutes();
    octet getUTCMonth();
    unsigned long getUTCSeconds();
    unsigned long setDate(long date);
    unsigned long setFullYear(long year, optional long month, optional long date);
    unsigned long setHours(long hour, optional long min, optional long sec, optional long ms);
    unsigned long setMilliseconds(long ms);
    unsigned long setMinutes(long min, optional long sec, optional long ms);
    unsigned long setMonth(long month, optional long date);
    unsigned long setSeconds(long sec, optional long ms);
    unsigned long setTime(long time);
    unsigned long setUTCDate(long date);
    unsigned long setUTCFullYear(long year, optional long month, optional long date);
    unsigned long setUTCHours(long hour, optional long min, optional long sec, optional long ms);
    unsigned long setUTCMilliseconds(long ms);
    unsigned long setUTCMinutes(long min, optional long sec, optional long ms);
    unsigned long setUTCMonth(long month, optional long date);
    unsigned long setUTCSeconds(long sec, optional long ms);
    DOMString toDateString();
    DOMString toISOString();
    DOMString toJSON();
    DOMString toLocaleDateString(optional DOMString locales, optional object options);
    DOMString toLocaleString(optional DOMString locales, optional object options);
    DOMString toLocaleTimeString(optional DOMString locales, optional object options);
    DOMString toString();
    DOMString toTimeString();
    DOMString toUTCString();
    unsigned long valueOf();
}

[Constructor(optional DOMString message)]
interface Error {
    attribute DOMString message;
    attribute DOMString name;
    DOMString toString();
}

[Constructor(optional DOMString message)]
interface EvalError : Error {
}

[Constructor, Constructor(any... argList), Constructor(unsigned long length), Constructor(Float32Array array), Constructor(object obj), Constructor(object buffer, optional long byteOffset, optional unsigned long byteLength)]
interface Float32Array {
}
Float32Array implements TypedArray;

[Constructor, Constructor(any... argList), Constructor(unsigned long length), Constructor(Float64Array array), Constructor(object obj), Constructor(object buffer, optional long byteOffset, optional unsigned long byteLength)]
interface Float64Array {
}
Float64Array implements TypedArray;

[Constructor(optional DOMString... params, optional DOMString functionBody)]
interface Function {
    static attribute long length; // initial value is 1
    any apply(Function thisArg, sequence<any> argArray);
    Function bind(Function thisArg, any... args);
    any call(Function thisArg, any... args);
    DOMString toString();
    attribute DOMString name;
    attribute unsigned long length;
};

[Constructor, Constructor(any... argList), Constructor(unsigned long length), Constructor(Int8Array array), Constructor(object obj), Constructor(object buffer, optional long byteOffset, optional unsigned long byteLength)]
interface Int8Array {
}
Int8Array implements TypedArray;

[Constructor, Constructor(any... argList), Constructor(unsigned long length), Constructor(Int16Array array), Constructor(object obj), Constructor(object buffer, optional long byteOffset, optional unsigned long byteLength)]
interface Int16Array {
}
Int16Array implements TypedArray;

[Constructor, Constructor(any... argList), Constructor(unsigned long length), Constructor(Int32Array array), Constructor(object obj), Constructor(object buffer, optional long byteOffset, optional unsigned long byteLength)]
interface Int32Array {
}
Int32Array implements TypedArray;

[Constructor(optional object iterable)]
interface Map {
    void clear();
    boolean delete(any key);
    object entries();
    void forEach(object callbackfn, optional any thisArg);
    void get(any key);
    boolean has(any key);
    object keys();
    Map set(any key, any value);
    attribute unsinged long size;
    object values();
}

[Constructor(optional long value)]
interface Number {
    const double EPSILON = 2.2204460492503130808472633361816e-16;
    const unsigned long long MAX_SAFE_INTEGER = 9007199254740991;
    const double MAX_VALUE = 1.7976931348623157e308;
    const long long MIN_SAFE_INTEGER = −9007199254740991;
    const double MIN_VALUE = 5e-324;
    const float NEGATIVE_INFINITY = -Infinity;
    const float POSITIVE_INFINITY = Infinity;
    const float NaN = NaN;
    static boolean isFinite(any number);
    static boolean isInteger(any number);
    static boolean iNaN(any number);
    static boolean isSafeInteger(any number);
    static float parseFloat(DOMString str);
    static long parseInt(DOMString str, long radix);
    DOMString toExponential(long fractionDigits);
    DOMString toFixed(long fractionDigits);
    DOMString toLocaleString(optional DOMString locales, optional object options);
    DOMString toPrecision(long precision);
    DOMString toString(long radix);
    object valueOf();
}

[Constructor(optional any value)]
interface Object {
    static attribute unsinged long length;
    static object assign(any target, optional any... sources);
    static object create(object obj, optional PropertyDescriptors properties);
    static object defineProperties(object obj, optional PropertyDescriptors properties);
    static object defineProperty(object obj, DOMString propertyKey, optional PropertyDescriptors properties);
    static object freeze(object obj);
    static PropertyDescriptor getOwnPropertyDescriptor(object obj, DOMString propertyKey);
    static sequence<DOMString> getOwnPropertyNames(object obj);
    static sequence<DOMString> getOwnPropertySymbols(object obj);
    static object getPrototypeOf(object obj);
    static boolean is(any value1, any value2);
    static boolean isExtensible(object obj);
    static boolean isFrozen(object obj);
    static boolean isSealed(object obj);
    static sequence<DOMString> keys(object obj);
    static object preventExtensions(object obj);
    static attribute Object prototype;
    object seal(object obj);
    object setPrototypeOf(object obj, object prototype);
    boolean hasOwnProperty(DOMString property);
    boolean isPrototypeOf(object value);
    boolean propertyIsEnumerable(DOMString property);
    DOMString toLocaleString();
    DOMString toString();
    object valueOf();
};

typedef dictionary<DOMString, PropertyDescriptor> PropertyDescriptors;
typedef (AccessorPropertyDescriptor or DataPropertyDescriptor) PropertyDescriptor;

dictionary AccessorPropertyDescriptor {
    Function get;
    Function set;
    required boolean configurable;
    required boolean enumerable;
};

dictionary DataPropertyDescriptor {
    required boolean configurable;
    required boolean enumerable;
    required boolean writable;
    required any value;
};

[Constructor(target, handler)]
interface Proxy {
    static object revocable(object target, object handler);
}

[Constructor(object executor)]
interface Promise {
    static Promise all(onject iterable);
    static Promise race(object iterable);
    static Promise reject(object r);
    static Promise resolve(object x);
    object catch(object onRejected);
    Promise then(object onFulfilled, object onReject);
}

[Constructor(optional DOMString message)]
interface RangeError : Error {
}

[Constructor(optional DOMString message)]
interface ReferenceError : Error {
}

[Constructor(object pattern, flags)]
interface RegExp {
    Array exec(DOMString string);
    attribute DOMString flags;
    attribute boolean global;
    attribute boolean ignoreCase;
    attribute boolean multiline;
    attribute DOMString source;
    attribute boolean sticky;
    boolean test(DOMString s);
    DOMString toString();
    attribute boolean unicode;
}

[Constructor(optional iterable)]
interface Set{
    object add(any value);
    void clear();
    boolean delete(any value);
    object entries();
    void forEach(object callbackfn, optional any thisArg);
    boolean has(any value);
    object keys();
    attribute unsigned long size;
    object values();
}

interface SIMD {
    static attribute float Float32x4;
    static attribute long Int32x4;
    static attribute short Int16x8;
    static attribute byte Int8x16;
    static attribute unsigned long Uint32x4;
    static attribute octet Uint8x16;
    static attribute boolean Bool32x4;
    static attribute boolean Bool16x8;
    static attribute boolean Bool8x16;
}

[Constructor(any value)]
interface String {
    static DOMString fromCharCode(DOMString... codeUnits);
    static DOMString fromCodePoint(DOMString... codePoints);
    static DOMString raw(DOMString template, DOMString... substitutions);
    DOMString charAt(long long pos);
    DOMString charCodeAt(long long pos);
    DOMString codePointAt(long long pos);
    DOMString concat(any... args);
    boolean endsWith(DOMString searchString, optional long long endPosition);
    boolean includes(DOMString searchString, optional long long position);
    long long indexOf(DOMString searchString, optional long long position);
    long long lastIndexOf(DOMString searchString, optional long long position);
    long localeCompare(object that, optional DOMString locales, optional object object options);
    object match(object regexp);
    DOMString normalize(optional DOMString form);
    DOMString repeat(long long count);
    DOMString replace(object searchValue, object replaceValue);
    object search(object regexp);
    DOMString slice(long long start, long long end);
    Array split(DOMString separator, long long limit);
    boolean startsWith(DOMString searchString, optional long long position);
    DOMString substring(long long start, long long end);
    DOMString toLocaleLowerCase(optional DOMString locales);
    DOMString toLocaleUpperCase(optional DOMString locales);
    DOMString toLowerCase();
    DOMString toString();
    DOMString toUpperCase();
    DOMString trim();
    object valueOf();
}

[Constructor(optional DOMString description)]
interface Symbol {
    static Symbol for(DOMString key);
    static attribute boolean hasInstance;
    static attribute booean isConcatSpreadable;
    static attribute sequence<Symbol> iterator;
    static DOMString keyFor(DOMString sym);
    static attribute sequence<DOMString> match;
    static attribute DOMString replace;
    static attribute unsigned long search;
    static attribute Function species;
    static attribute sequence<DOMString> split;
    static attribute object toPrimitive;
    static attribute DOMString toStringTag;
    static attribute object unscopables;
    DOMString toString();
    object valueOf();
}

[Constructor(optional DOMString message)]
interface SyntaxError : Error {
}

[NoInterFaceObject]
interface TypedArray {
    static object from(object source, optional object mapfn, optional any thisArg);
    static object of(any... items);
    attribute object buffer;
    attribute unsigned long byteLength;
    attribute unsigned long byteOffset;
    object copyWithin(object target, unsigned long start, optional unsigned long end);
    object entries();
    boolean every(object callbackfn, optional any thisArg);
    object fill(object value, optional long start, optional long end);
    object filter(object callbackfn, optional any thisArg);
    void find(object predicate, optional any thisArg);
    long findIndex(object predicate, optional any thisArg);
    void forEach(object callbackfn, optional any thisArg);
    long indexOf(any searchElement, optional long fromIndex);
    DOMString join(DOMString separator);
    object keys();
    long lastIndexOf(any searchElement, optional long fromIndex);
    attribute unsigned long length;
    object map(object callbackfn, optional any thisArg);
    object reduce(object callbackfn, optional any initialValue);
    object reduceRight(object callbackfn, optional any initialValue);
    object reverse();
    void set(Array array, optional long offset);
    void set(object typedArray, optional long offset);
    object slice(long start, long end);
    boolean some(object callbackfn, optional any thisArg);
    void sort(object comparefn);
    object subarray(optional long begin, optional long end);
    DOMString toLocaleString();
    DOMString toString();
    object values();
    attribute unsigned long BYTES_PER_ELEMENT;
}

[Constructor(optional DOMString message)]
interface TypeError : Error {
}

[Constructor, Constructor(any... argList), Constructor(octet length), Constructor(Uint8Array array), Constructor(object obj), Constructor(object buffer, optional octet byteOffset, optional octet byteLength)]
interface Uint8Array {
}
Uint8Array implements TypedArray;

[Constructor, Constructor(any... argList), Constructor(octet length), Constructor(Uint8ClampedArray array), Constructor(object obj), Constructor(object buffer, optional octet byteOffset, optional octet byteLength)]
interface Uint8ClampedArray {
}
Uint8ClampedArray implements TypedArray;

[Constructor, Constructor(any... argList), Constructor(unsigned short length), Constructor(Uint16Array array), Constructor(object obj), Constructor(object buffer, optional unsigned short byteOffset, optional unsigned short byteLength)]
interface Uint16Array {
}
Uint16Array implements TypedArray;

[Constructor, Constructor(any... argList), Constructor(unsigned long length), Constructor(Uint32Array array), Constructor(object obj), Constructor(object buffer, optional unsigned long byteOffset, optional unsigned long byteLength)]
interface Uint32Array {
}
Uint32Array implements TypedArray;

[Constructor(optional DOMString message)]
interface URIError : Error {
}

[Constructor(optional object iterable)]
interface WeakMap {
    boolean delete(any key);
    void get(any key);
    boolean has(kany ey);
    WeakMap set(any key, any value);
}

[Constructor(optional object iterable)]
interface WeakSet {
    Weakset add(any value);
    boolean delete(any value);
    boolean has(any value);
}

interface JSON {
    static object parse(DOMString text, optional object reviver);
    DOMString stringify(object value, optional object replacer, optional object space);
}

interface Math {
    const double E = 2.7182818284590452354;
    const double LN10 = 2.302585092994046;
    const double LN2 = 0.6931471805599453;
    const double LOG10E = 0.4342944819032518;
    const double LOG2E = 1.4426950408889634;
    const double PI = 3.1415926535897932;
    const double SQRT1_2 = 0.7071067811865476;
    const double SQRT2 = 1.4142135623730951;
    static float abs(float x);
    static float acos(float x);
    static float acosh(float x);
    static float asin(float x);
    static float asinh(float x);
    static float atan(float x);
    static float atanh(float x);
    static float atan2(float y, float x);
    static float cbrt(float x);
    static float ceil(float x);
    static float clz32(float x);
    static float cos(float x);
    static float cosh(float x);
    static float exp(float x);
    static float expm1(float x);
    static float floor(float x);
    static float fround(float x);
    static float hypot(float value1, float value2, float values...);
    static float imul(float x, float long y);
    static float log(float x);
    static float log1p(float x);
    static float log10(float x);
    static float log2(float x);
    static float max(float value1, float value2, float values...);
    static float min(float value1, float value2, float values...);
    static float pow(float x, float y);
    static float random();
    static float round(float x);
    static float sign(float x);
    static float sin(float x);
    static float sinh(float x);
    static float sqrt(float x);
    static float tan(float x);
    static float tanh(float x);
    static float trunc(float x);
}

interface Intl {
    static attribute object Collator;
    static attribute object DateTimeFormat;
    static attribute object NumberFormat;
}