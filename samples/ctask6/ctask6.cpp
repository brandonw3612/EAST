#include <iostream>
#include <vector>
#include <unordered_set>

using namespace std;

int main() {
    int N;
    cin >> N;
    unordered_set<int> digits;
    for (int i = 0; i < N; ++i) {
        int d;
        cin >> d;
        digits.insert(d);
    }

    vector<int> ABCs;
    for (int a : digits) {
        if (a == 0) continue;
        for (int b : digits) {
            for (int c : digits) {
                ABCs.push_back(a * 100 + b * 10 + c);
            }
        }
    }

    vector<int> DEs;
    for (int d : digits) {
        if (d == 0) continue;
        for (int e : digits) {
            DEs.push_back(d * 10 + e);
        }
    }

    int count = 0;

    for (int abc : ABCs) {
        for (int de : DEs) {
            int e = de % 10;
            int d = de / 10;

            int p1 = abc * e;
            if (p1 < 100 || p1 >= 1000) continue;
            int p1_a = p1 / 100;
            int p1_b = (p1 / 10) % 10;
            int p1_c = p1 % 10;
            if (!digits.count(p1_a) || !digits.count(p1_b) || !digits.count(p1_c)) {
                continue;
            }

            int p2 = abc * d;
            if (p2 < 100 || p2 >= 1000) continue;
            int p2_a = p2 / 100;
            int p2_b = (p2 / 10) % 10;
            int p2_c = p2 % 10;
            if (!digits.count(p2_a) || !digits.count(p2_b) || !digits.count(p2_c)) {
                continue;
            }

            int tp = abc * de;
            if (tp < 1000 || tp >= 10000) continue;
            int tp_a = tp / 1000;
            int tp_b = (tp / 100) % 10;
            int tp_c = (tp / 10) % 10;
            int tp_d = tp % 10;
            if (!digits.count(tp_a) || !digits.count(tp_b) || !digits.count(tp_c) || !digits.count(tp_d)) {
                continue;
            }

            ++count;
        }
    }

    cout << count << endl;

    return 0;
}