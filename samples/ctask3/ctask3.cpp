#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>

using namespace std;

int main() {
    int n;
    cin >> n;
    vector<pair<int, int>> points;
    for (int i = 0; i < n; ++i) {
        int x, y;
        cin >> x >> y;
        points.emplace_back(x, y);
    }
    double max_dist = 0.0;
    for (int i = 0; i < n; ++i) {
        for (int j = i + 1; j < n; ++j) {
            int dx = points[i].first - points[j].first;
            int dy = points[i].second - points[j].second;
            double dist = sqrt(dx*dx + dy*dy);
            if (dist > max_dist) {
                max_dist = dist;
            }
        }
    }
    cout << fixed << setprecision(9) << max_dist << endl;
    return 0;
}