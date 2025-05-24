## split(Read) vs span slice(OptimizeRead)

| Method       | Mean     | Error    | StdDev   | Median   | Gen0   | Allocated |
|------------- |---------:|---------:|---------:|---------:|-------:|----------:|
| Read         | 318.0 ns |  6.40 ns | 17.09 ns | 312.1 ns | 0.0930 |     489 B |
| OptimizeRead | 468.7 ns | 12.93 ns | 38.14 ns | 464.6 ns | 0.0439 |     232 B |

## split(Read) vs span slice(OptimizeRead)
## 反射寫入List物件

| Method       | Mean     | Error     | StdDev    | Gen0   | Allocated |
|------------- |---------:|----------:|----------:|-------:|----------:|
| Read         | 2.196 us | 0.0433 us | 0.0914 us | 0.1526 |     801 B |
| OptimizeRead | 2.013 us | 0.0392 us | 0.0550 us | 0.1030 |     545 B |


## split(Read) vs span slice(OptimizeRead)
## Expression + SetterDelegate
| Method       | Mean       | Error    | StdDev   | Gen0   | Allocated |
|------------- |-----------:|---------:|---------:|-------:|----------:|
| Read         | 2,128.2 ns | 42.07 ns | 62.97 ns | 0.1526 |     801 B |
| OptimizeRead |   639.5 ns | 14.88 ns | 43.63 ns | 0.0534 |     280 B |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
| Method        | Mean       | Error    | StdDev   | Gen0   | Allocated |
|-------------- |-----------:|---------:|---------:|-------:|----------:|
| Write         | 1,454.0 ns | 28.46 ns | 23.76 ns | 0.1221 |     641 B |
| OptimizeWrite |   434.0 ns |  8.61 ns | 11.79 ns | 0.1144 |     605 B |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
## 300萬次
| Method        | Mean    | Error    | StdDev   | Gen0        | Allocated |
|-------------- |--------:|---------:|---------:|------------:|----------:|
| Write         | 4.654 s | 0.0903 s | 0.1004 s | 366000.0000 |   1.79 GB |
| OptimizeWrite | 1.276 s | 0.0221 s | 0.0207 s | 346000.0000 |   1.69 GB |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
## stringBuilder.Append(a.ToString() + ",");
| Method        | Mean       | Error    | StdDev   | Gen0   | Allocated |
|-------------- |-----------:|---------:|---------:|-------:|----------:|
| Write         | 1,540.3 ns | 30.67 ns | 60.54 ns | 0.1221 |     641 B |
| OptimizeWrite |   316.3 ns |  6.31 ns | 11.70 ns | 0.0587 |     510 B |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
## stringBuilder.Append(a);
## stringBuilder.Append(",");
| Method        | Mean       | Error    | StdDev   | Gen0   | Allocated |
|-------------- |-----------:|---------:|---------:|-------:|----------:|
| Write         | 1,540.3 ns | 30.67 ns | 60.54 ns | 0.1221 |     641 B |
| OptimizeWrite |   316.3 ns |  6.31 ns | 11.70 ns | 0.0587 |     308 B |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
## stringBuilder.Append(a);
## stringBuilder.Append(',');
| Method        | Mean       | Error    | StdDev   | Gen0   | Allocated |
|-------------- |-----------:|---------:|---------:|-------:|----------:|
| Write         | 1,595.5 ns | 31.86 ns | 79.33 ns | 0.1221 |     641 B |
| OptimizeWrite |   304.7 ns |  5.61 ns | 11.08 ns | 0.0587 |     308 B |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
```csharp
for (int i = 0; i < propsCount; i++)
{
    object a = getters[i](info);
    stringBuilder.Append(a);
    stringBuilder.Append(',');
}

string data = stringBuilder.ToString().TrimEnd(',');

--Optimize--
for (int i = 0; i < propsCount; i++)
{
    object a = getters[i](info);
    stringBuilder.Append(a);
    if (i < propsCount - 1)
        stringBuilder.Append(',');
}
string data = stringBuilder.ToString();

```
| Method        | Mean       | Error    | StdDev   | Gen0   | Allocated |
|-------------- |-----------:|---------:|---------:|-------:|----------:|
| Write         | 1,473.6 ns | 28.47 ns | 37.03 ns | 0.1221 |     641 B |
| OptimizeWrite |   218.6 ns |  4.27 ns |  7.36 ns | 0.0305 |     160 B |

## GetValue(Write) vs Expression + SetterDelegate(OptimizeWrite)
## stringBuilder.CopyTo(0, buffer, 0, stringBuilder.Length - 1);
| Method        | Mean       | Error    | StdDev   | Gen0   | Allocated |
|-------------- |-----------:|---------:|---------:|-------:|----------:|
| Write         | 1,487.9 ns | 29.46 ns | 31.52 ns | 0.1221 |     641 B |
| OptimizeWrite |   199.4 ns |  2.54 ns |  2.25 ns | 0.0060 |      32 B |
