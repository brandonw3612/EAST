#include <iostream>
#include <vector>

using namespace std;

int main() {
    int n;
    cin >> n;
    vector<int> a(n);
    for (int i = 0; i < n; ++i) {
        cin >> a[i];
    }
    vector<int> res(n);
    for (int i = 0; i < n; ++i) {
        res[i] = a[i] + (i == 0 ? a.back() : a[i - 1]);
    }
    for (int i = 0; i < n; ++i) {
        if (i > 0) cout << ' ';
        cout << res[i];
    }
    return 0;
}