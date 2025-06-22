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

const int MAX_LEVEL = 35;

int main() {
    ios_base::sync_with_stdio(false);
    cin.tie(nullptr);

    int n;
    long long k;
    cin >> n >> k;

    vector<int> f(n), w(n);
    for (int& x : f) cin >> x;
    for (int& x : w) cin >> x;

    vector<vector<int>> jump(MAX_LEVEL, vector<int>(n));
    vector<vector<long long>> sum(MAX_LEVEL, vector<long long>(n));
    vector<vector<int>> min_val(MAX_LEVEL, vector<int>(n));

    for (int i = 0; i < n; ++i) {
        jump[0][i] = f[i];
        sum[0][i] = w[i];
        min_val[0][i] = w[i];
    }

    for (int j = 1; j < MAX_LEVEL; ++j) {
        for (int i = 0; i < n; ++i) {
            int prev = jump[j-1][i];
            jump[j][i] = jump[j-1][prev];
            sum[j][i] = sum[j-1][i] + sum[j-1][prev];
            min_val[j][i] = min(min_val[j-1][i], min_val[j-1][prev]);
        }
    }

    for (int i = 0; i < n; ++i) {
        long long s = 0;
        int m = INT_MAX;
        int node = i;
        for (int j = 0; j < MAX_LEVEL; ++j) {
            if (k & (1LL << j)) {
                s += sum[j][node];
                m = min(m, min_val[j][node]);
                node = jump[j][node];
            }
        }
        cout << s << " " << m << "\n";
    }
}