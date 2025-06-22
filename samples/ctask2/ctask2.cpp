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

int main() {
    ios_base::sync_with_stdio(false);
    cin.tie(nullptr);
    int N;
    cin >> N;
    vector<pair<int, int>> events;
    events.reserve(N);
    for (int i = 0; i < N; ++i) {
        int S, D;
        cin >> S >> D;
        events.emplace_back(S + D - 1, S);
    }
    sort(events.begin(), events.end());
    int count = 0, last_end = 0;
    for (auto& [end, start] : events) {
        if (start > last_end) {
            ++count;
            last_end = end;
        }
    }
    cout << count << "\n";
    return 0;
}