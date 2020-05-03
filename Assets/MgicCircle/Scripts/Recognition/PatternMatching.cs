using UnityEngine;

public class PatternMatching {
    public MagicCircle MagicCircle { get; set; }
    public short[][] target2DArray { get; set; }

    public enum Mode {
        SAD,
        OTHER,
    }

    public PatternMatching(MagicCircle magicCircle, short[][] target2DArray) {
        this.MagicCircle = magicCircle;
        this.target2DArray = target2DArray;
    }

    public bool IsMatch(Mode mode = Mode.SAD) {
        var result = false;

        switch (mode) {
            case Mode.SAD:
                var value = this.SAD();
                DebugLogger.Log("result = " + value);
                result = value <= this.MagicCircle.SuccessThreshold;
                break;
            case Mode.OTHER:
                break;
            default:
                break;
        }

        return result;
    }

    // テンプレートと誤差を計算する。SAD(Sum of Absolute Difference)
    private int SAD() {
        var result = 0;
        for (int y = 0; y < this.MagicCircle.Template.Length; y++) {
            for (int x = 0; x < this.MagicCircle.Template[0].Length; x++) {
                result += Mathf.Abs(this.target2DArray[y][x] - this.MagicCircle.Template[y][x]);
            }
        }

        return result;
    }
}
