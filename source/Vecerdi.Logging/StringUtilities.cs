using System.Runtime.CompilerServices;

namespace Vecerdi.Logging;

public static class StringUtilities {
    /// <summary>
    /// Concatenates two <see cref="T:System.ReadOnlySpan`1"/> instances and returns a new string.
    /// </summary>
    /// <param name="span1">The first <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span2">The second <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <returns>A new string containing the characters of both input ReadOnlySpan instances.</returns>
    public static unsafe string Concat(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2) {
        string result = new('\0', span1.Length + span2.Length);

        fixed (char* src1 = span1, src2 = span2)
        fixed (char* target = result) {
            Unsafe.CopyBlock(target, src1, (uint)span1.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length, src2, (uint)span2.Length * sizeof(char));
        }

        return result;
    }

    /// <summary>
    /// Concatenates three <see cref="T:System.ReadOnlySpan`1"/> instances and returns a new string.
    /// </summary>
    /// <param name="span1">The first <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span2">The second <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span3">The third <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <returns>A new string containing the characters of all three input ReadOnlySpan instances.</returns>
    public static unsafe string Concat(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2, ReadOnlySpan<char> span3) {
        string result = new('\0', span1.Length + span2.Length + span3.Length);

        fixed (char* src1 = span1, src2 = span2, src3 = span3)
        fixed (char* target = result) {
            Unsafe.CopyBlock(target, src1, (uint)span1.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length, src2, (uint)span2.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length + span2.Length, src3, (uint)span3.Length * sizeof(char));
        }

        return result;
    }

    /// <summary>
    /// Concatenates four <see cref="T:System.ReadOnlySpan`1"/> instances and returns a new string.
    /// </summary>
    /// <param name="span1">The first <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span2">The second <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span3">The third <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span4">The fourth <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <returns>A new string containing the characters of all four input ReadOnlySpan instances.</returns>
    public static unsafe string Concat(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2, ReadOnlySpan<char> span3, ReadOnlySpan<char> span4) {
        string result = new('\0', span1.Length + span2.Length + span3.Length + span4.Length);

        fixed (char* src1 = span1, src2 = span2, src3 = span3, src4 = span4)
        fixed (char* target = result) {
            Unsafe.CopyBlock(target, src1, (uint)span1.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length, src2, (uint)span2.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length + span2.Length, src3, (uint)span3.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length + span2.Length + span3.Length, src4, (uint)span4.Length * sizeof(char));
        }

        return result;
    }

    /// <summary>
    /// Concatenates five <see cref="T:System.ReadOnlySpan`1"/> instances and returns a new string.
    /// </summary>
    /// <param name="span1">The first <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span2">The second <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span3">The third <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span4">The fourth <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span5">The fifth <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <returns>A new string containing the characters of all five input ReadOnlySpan instances.</returns>
    public static unsafe string Concat(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2, ReadOnlySpan<char> span3, ReadOnlySpan<char> span4, ReadOnlySpan<char> span5) {
        string result = new('\0', span1.Length + span2.Length + span3.Length + span4.Length + span5.Length);

        fixed (char* src1 = span1, src2 = span2, src3 = span3, src4 = span4, src5 = span5)
        fixed (char* target = result) {
            Unsafe.CopyBlock(target, src1, (uint)span1.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length, src2, (uint)span2.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length + span2.Length, src3, (uint)span3.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length + span2.Length + span3.Length, src4, (uint)span4.Length * sizeof(char));
            Unsafe.CopyBlock(target + span1.Length + span2.Length + span3.Length + span4.Length, src5, (uint)span5.Length * sizeof(char));
        }

        return result;
    }

    /// <summary>
    /// Concatenates six <see cref="T:System.ReadOnlySpan`1"/> instances and returns a new string.
    /// </summary>
    /// <param name="span1">The first <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span2">The second <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span3">The third <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span4">The fourth <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span5">The fifth <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <param name="span6">The sixth <see cref="T:System.ReadOnlySpan`1"/> to concatenate.</param>
    /// <returns>A new string containing the characters of all six input ReadOnlySpan instances.</returns>
    public static unsafe string Concat(
        ReadOnlySpan<char> span1, ReadOnlySpan<char> span2, ReadOnlySpan<char> span3,
        ReadOnlySpan<char> span4, ReadOnlySpan<char> span5, ReadOnlySpan<char> span6
    ) {
        string result = new('\0', span1.Length + span2.Length + span3.Length + span4.Length + span5.Length + span6.Length);

        fixed (char* src1 = span1, src2 = span2, src3 = span3, src4 = span4, src5 = span5, src6 = span6)
        fixed (char* target = result) {
            char* targetPtr = target;
            Unsafe.CopyBlock(targetPtr, src1, (uint)span1.Length * sizeof(char));
            targetPtr += span1.Length;
            Unsafe.CopyBlock(targetPtr, src2, (uint)span2.Length * sizeof(char));
            targetPtr += span2.Length;
            Unsafe.CopyBlock(targetPtr, src3, (uint)span3.Length * sizeof(char));
            targetPtr += span3.Length;
            Unsafe.CopyBlock(targetPtr, src4, (uint)span4.Length * sizeof(char));
            targetPtr += span4.Length;
            Unsafe.CopyBlock(targetPtr, src5, (uint)span5.Length * sizeof(char));
            targetPtr += span5.Length;
            Unsafe.CopyBlock(targetPtr, src6, (uint)span6.Length * sizeof(char));
        }

        return result;
    }

    public static unsafe string Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0) {
        int length = 0;
        int argIndex = 0;
        for (int i = 0; i < format.Length; i++) {
            if (argIndex < 1 && format[i] == '{' && i + 1 < format.Length && format[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                length += argIndex switch {
                    0 => arg0.Length,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                argIndex++;
                i++;
            } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                // Skip escaped braces.
            } else {
                length++;
            }
        }

        string result = new('\0', length);
        int resultIndex = 0;
        argIndex = 0;

        fixed (char* formatPtr = format, resultPtr = result) {
            for (int i = 0; i < format.Length; i++) {
                if (argIndex < 1 && formatPtr[i] == '{' && i + 1 < format.Length && formatPtr[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                    ReadOnlySpan<char> argString = argIndex switch {
                        0 => arg0,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    fixed (char* argPtr = argString) {
                        Unsafe.CopyBlock(resultPtr + resultIndex, argPtr, (uint)argString.Length * sizeof(char));
                    }

                    resultIndex += argString.Length;
                    argIndex++;
                    i++;
                } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                    // Skip escaped braces.
                } else {
                    resultPtr[resultIndex++] = formatPtr[i];
                }
            }
        }

        return result;
    }

    public static unsafe string Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1) {
        int length = 0;
        int argIndex = 0;
        for (int i = 0; i < format.Length; i++) {
            if (argIndex < 2 && format[i] == '{' && i + 1 < format.Length && format[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                length += argIndex switch {
                    0 => arg0.Length,
                    1 => arg1.Length,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                argIndex++;
                i++;
            } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                // Skip escaped braces.
            } else {
                length++;
            }
        }

        string result = new('\0', length);
        int resultIndex = 0;
        argIndex = 0;

        fixed (char* formatPtr = format, resultPtr = result) {
            for (int i = 0; i < format.Length; i++) {
                if (argIndex < 2 && formatPtr[i] == '{' && i + 1 < format.Length && formatPtr[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                    ReadOnlySpan<char> argString = argIndex switch {
                        0 => arg0,
                        1 => arg1,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    fixed (char* argPtr = argString) {
                        Unsafe.CopyBlock(resultPtr + resultIndex, argPtr, (uint)argString.Length * sizeof(char));
                    }

                    resultIndex += argString.Length;
                    argIndex++;
                    i++;
                } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                    // Skip escaped braces.
                } else {
                    resultPtr[resultIndex++] = formatPtr[i];
                }
            }
        }

        return result;
    }

    public static unsafe string Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2) {
        int length = 0;
        int argIndex = 0;
        for (int i = 0; i < format.Length; i++) {
            if (argIndex < 3 && format[i] == '{' && i + 1 < format.Length && format[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                length += argIndex switch {
                    0 => arg0.Length,
                    1 => arg1.Length,
                    2 => arg2.Length,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                argIndex++;
                i++;
            } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                // Skip escaped braces.
            } else {
                length++;
            }
        }

        string result = new('\0', length);
        int resultIndex = 0;
        argIndex = 0;

        fixed (char* formatPtr = format, resultPtr = result) {
            for (int i = 0; i < format.Length; i++) {
                if (argIndex < 3 && formatPtr[i] == '{' && i + 1 < format.Length && formatPtr[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                    ReadOnlySpan<char> argString = argIndex switch {
                        0 => arg0,
                        1 => arg1,
                        2 => arg2,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    fixed (char* argPtr = argString) {
                        Unsafe.CopyBlock(resultPtr + resultIndex, argPtr, (uint)argString.Length * sizeof(char));
                    }

                    resultIndex += argString.Length;
                    argIndex++;
                    i++;
                } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                    // Skip escaped braces.
                } else {
                    resultPtr[resultIndex++] = formatPtr[i];
                }
            }
        }

        return result;
    }

    public static unsafe string Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3) {
        int length = 0;
        int argIndex = 0;
        for (int i = 0; i < format.Length; i++) {
            if (argIndex < 4 && format[i] == '{' && i + 1 < format.Length && format[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                length += argIndex switch {
                    0 => arg0.Length,
                    1 => arg1.Length,
                    2 => arg2.Length,
                    3 => arg3.Length,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                argIndex++;
                i++;
            } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                // Skip escaped braces.
            } else {
                length++;
            }
        }

        string result = new('\0', length);
        int resultIndex = 0;
        argIndex = 0;

        fixed (char* formatPtr = format, resultPtr = result) {
            for (int i = 0; i < format.Length; i++) {
                if (argIndex < 4 && formatPtr[i] == '{' && i + 1 < format.Length && formatPtr[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                    ReadOnlySpan<char> argString = argIndex switch {
                        0 => arg0,
                        1 => arg1,
                        2 => arg2,
                        3 => arg3,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    fixed (char* argPtr = argString) {
                        Unsafe.CopyBlock(resultPtr + resultIndex, argPtr, (uint)argString.Length * sizeof(char));
                    }

                    resultIndex += argString.Length;
                    argIndex++;
                    i++;
                } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                    // Skip escaped braces.
                } else {
                    resultPtr[resultIndex++] = formatPtr[i];
                }
            }
        }

        return result;
    }

    public static unsafe string Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> arg4) {
        int length = 0;
        int argIndex = 0;
        for (int i = 0; i < format.Length; i++) {
            if (argIndex < 5 && format[i] == '{' && i + 1 < format.Length && format[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                length += argIndex switch {
                    0 => arg0.Length,
                    1 => arg1.Length,
                    2 => arg2.Length,
                    3 => arg3.Length,
                    4 => arg4.Length,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                argIndex++;
                i++;
            } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                // Skip escaped braces.
            } else {
                length++;
            }
        }

        string result = new('\0', length);
        int resultIndex = 0;
        argIndex = 0;

        fixed (char* formatPtr = format, resultPtr = result) {
            for (int i = 0; i < format.Length; i++) {
                if (argIndex < 5 && formatPtr[i] == '{' && i + 1 < format.Length && formatPtr[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                    ReadOnlySpan<char> argString = argIndex switch {
                        0 => arg0,
                        1 => arg1,
                        2 => arg2,
                        3 => arg3,
                        4 => arg4,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    fixed (char* argPtr = argString) {
                        Unsafe.CopyBlock(resultPtr + resultIndex, argPtr, (uint)argString.Length * sizeof(char));
                    }

                    resultIndex += argString.Length;
                    argIndex++;
                    i++;
                } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                    // Skip escaped braces.
                } else {
                    resultPtr[resultIndex++] = formatPtr[i];
                }
            }
        }

        return result;
    }

    public static unsafe string Format(ReadOnlySpan<char> format, params string?[] args) {
        if (args.Length == 0) {
            return format.ToString();
        }

        int argIndex = 0;
        int length = 0;
        for (int i = 0; i < format.Length; i++) {
            if (argIndex < args.Length && format[i] == '{' && i + 1 < format.Length && format[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                length += args[argIndex]?.Length ?? 0;
                argIndex++;
                i++;
            } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                // Skip escaped braces.
            } else {
                length++;
            }
        }

        string result = new('\0', length);
        int resultIndex = 0;
        argIndex = 0;

        fixed (char* formatPtr = format, resultPtr = result) {
            for (int i = 0; i < format.Length; i++) {
                if (argIndex < args.Length && formatPtr[i] == '{' && i + 1 < format.Length && formatPtr[i + 1] == '}' && (i == 0 || format[i - 1] != '{')) {
                    ReadOnlySpan<char> argString = args[argIndex] is { } arg ? arg.AsSpan() : default;
                    fixed (char* argPtr = argString) {
                        Unsafe.CopyBlock(resultPtr + resultIndex, argPtr, (uint)argString.Length * sizeof(char));
                    }

                    resultIndex += argString.Length;
                    argIndex++;
                    i++;
                } else if ((i > 0 && format[i - 1] == '{' && format[i] == '{') || (i + 1 < format.Length && format[i] == '}' && format[i + 1] == '}')) {
                    // Skip escaped braces.
                } else {
                    resultPtr[resultIndex++] = formatPtr[i];
                }
            }
        }

        return result;
    }
}