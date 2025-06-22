#include <vector>
#include <queue>
#include <climits>
#include <algorithm>
#include <string>
#include <iostream>

using namespace std;

struct Edge {
int to, rev, cap, cost;
};

vector<vector<Edge>> graph;
vector<int> potential;

void add_edge(int from, int to, int cap, int cost) {
graph[from].push_back({to, (int)graph[to].size(), cap, cost});
graph[to].push_back({from, (int)graph[from].size() - 1, 0, -cost});
}

pair<int, int> min_cost_flow(int source, int sink, int max_flow) {
int flow = 0, cost = 0;
potential.assign(graph.size(), 0);
vector<int> dist, prev_v, prev_e;

while (flow < max_flow) {
dist.assign(graph.size(), INT_MAX);
dist[source] = 0;
priority_queue<pair<int, int>, vector<pair<int, int>>, greater<pair<int, int>>> pq;
pq.push({0, source});
prev_v.assign(graph.size(), -1);
prev_e.assign(graph.size(), -1);

while (!pq.empty()) {
auto [d, u] = pq.top();
pq.pop();
if (d > dist[u]) continue;
for (size_t i = 0; i < graph[u].size(); ++i) {
Edge &e = graph[u][i];
if (e.cap > 0 && dist[e.to] > d + e.cost + potential[u] - potential[e.to]) {
dist[e.to] = d + e.cost + potential[u] - potential[e.to];
prev_v[e.to] = u;
prev_e[e.to] = i;
pq.push({dist[e.to], e.to});
}
}
}

if (dist[sink] == INT_MAX) break;

for (size_t v = 0; v < graph.size(); ++v)
potential[v] += dist[v];

int min_f = max_flow - flow;
for (int v = sink; v != source; v = prev_v[v])
min_f = min(min_f, graph[prev_v[v]][prev_e[v]].cap);

for (int v = sink; v != source; v = prev_v[v]) {
Edge &e = graph[prev_v[v]][prev_e[v]];
e.cap -= min_f;
graph[v][e.rev].cap += min_f;
cost += e.cost * min_f;
}
flow += min_f;
}
return {flow, cost};
}

int main() {
int n, m;
cin >> n >> m;

vector<string> s(n);
for (int i = 0; i < n; ++i)
cin >> s[i];

vector<vector<int>> a(n, vector<int>(m));
for (int i = 0; i < n; ++i)
for (int j = 0; j < m; ++j)
cin >> a[i][j];

int num_nodes = 1 + n + m * 26 + 1;
graph.resize(num_nodes);

int source = 0;
int sink = 1 + n + m * 26;

// Source to each string
for (int i = 0; i < n; ++i)
add_edge(source, 1 + i, 1, 0);

// Each (j, c) to sink
for (int j = 0; j < m; ++j)
for (char c = 'a'; c <= 'z'; ++c) {
int node = 1 + n + j * 26 + (c - 'a');
add_edge(node, sink, 1, 0);
}

// Strings to (j, c) pairs
for (int i = 0; i < n; ++i) {
for (int j = 0; j < m; ++j) {
for (char c = 'a'; c <= 'z'; ++c) {
int cost = (s[i][j] == c) ? 0 : a[i][j];
int to_node = 1 + n + j * 26 + (c - 'a');
add_edge(1 + i, to_node, 1, cost);
}
}
}

auto [flow, cost] = min_cost_flow(source, sink, n);
cout << cost << endl;

return 0;
}