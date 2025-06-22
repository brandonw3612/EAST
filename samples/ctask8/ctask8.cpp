#include <iostream>
#include <vector>
#include <algorithm>
#include <cmath>
#include <set>
#include <map>
#include <stack>
#include <queue>
#include <string>

using namespace std;

const int MOD = 1000000007;
const int MAX_BITS = 20;

int main() {
    ios_base::sync_with_stdio(false);
    cin.tie(nullptr);

    int n;
    cin >> n;
    vector<int> a(n);
    for (int& num : a) cin >> num;

    vector<int> C(1 << MAX_BITS, 0);
    for (int num : a) C[num]++;

    for (int bit = 0; bit < MAX_BITS; ++bit) {
        for (int mask = 0; mask < (1 << MAX_BITS); ++mask) {
            if (!(mask & (1 << bit))) {
                C[mask] += C[mask ^ (1 << bit)];
            }
        }
    }

    vector<int> pow2(n + 1, 1);
    for (int i = 1; i <= n; ++i) {
        pow2[i] = (pow2[i - 1] * 2LL) % MOD;
    }

    long long result = 0;
    for (int mask = 0; mask < (1 << MAX_BITS); ++mask) {
        int bits = __builtin_popcount(mask);
        int sign = (bits % 2 == 0) ? 1 : MOD - 1;
        int cnt = C[mask];
        int term = (pow2[cnt] - 1LL + MOD) % MOD;
        term = (term * 1LL * sign) % MOD;
        result = (result + term) % MOD;
    }

    cout << result << '\n';
    return 0;
}