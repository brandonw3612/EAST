#include <vector>
#include <climits>
#include <cstdio>
using namespace std;

int main() {
    int n;
    scanf("%d", &n);
    vector<long long> b(n+1), a(n+1);
    for (int i=1; i<=n; ++i) scanf("%lld", &b[i]);
    for (int i=1; i<=n; ++i) scanf("%lld", &a[i]);
    vector<int> parent(n+1);
    vector<long long> k(n+1);
    for (int i=2; i<=n; ++i) {
        scanf("%d%lld", &parent[i], &k[i]);
    }
    for (int i=n; i>=1; --i) {
        if (b[i] < a[i]) {
            if (i == 1) {
                printf("NO\n");
                return 0;
            }
            long long deficit = a[i] - b[i];
            int p = parent[i];
            if (k[i] > LLONG_MAX / deficit) {
                printf("NO\n");
                return 0;
            }
            long long needed = deficit * k[i];
            if (b[p] < needed) {
                printf("NO\n");
                return 0;
            }
            b[p] -= needed;
        } else {
            if (i != 1) {
                long long surplus = b[i] - a[i];
                int p = parent[i];
                b[p] += surplus;
            }
        }
    }
    printf("YES\n");
    return 0;
}