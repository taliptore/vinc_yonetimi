import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../services/api_service.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _api = ApiService();
  Map<String, dynamic>? _data;
  String? _error;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    try {
      final d = await _api.getDashboard();
      if (mounted) setState(() => _data = d);
    } catch (e) {
      if (mounted) setState(() => _error = e.toString().replaceFirst('Exception: ', ''));
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Dashboard'),
        actions: [
          IconButton(icon: const Icon(Icons.logout), onPressed: () async {
            final nav = Navigator.of(context);
            await context.read<AuthProvider>().logout();
            if (!mounted) return;
            nav.pushReplacementNamed('/login');
          }),
        ],
      ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(child: Text(_error!, textAlign: TextAlign.center))
              : RefreshIndicator(
                  onRefresh: _load,
                  child: ListView(
                    padding: const EdgeInsets.all(16),
                    children: [
                      if (_data?['todayJob'] != null)
                        Card(
                          child: ListTile(
                            title: const Text('Bugünkü iş'),
                            subtitle: Text(_data!['todayJob']['site']?.toString() ?? _data!['todayJob']['plate']?.toString() ?? '-'),
                            trailing: const Icon(Icons.construction),
                          ),
                        ),
                      if (_data?['totalHoursToday'] != null)
                        Card(
                          child: ListTile(
                            title: const Text('Bugünkü çalışma süresi'),
                            subtitle: Text('${_data!['totalHoursToday']} saat'),
                          ),
                        ),
                      if (_data == null || (_data!['todayJob'] == null && _data!['totalHoursToday'] == null))
                        const Card(
                          child: ListTile(
                            title: Text('Bugün atanmış iş yok'),
                            subtitle: Text('İş listesinden işe başlayabilirsiniz.'),
                          ),
                        ),
                      const SizedBox(height: 24),
                      ListTile(
                        leading: const Icon(Icons.list),
                        title: const Text('İş listesi'),
                        onTap: () => Navigator.pushNamed(context, '/jobs'),
                      ),
                      ListTile(
                        leading: const Icon(Icons.warning_amber),
                        title: const Text('Arıza bildirimi'),
                        onTap: () => Navigator.pushNamed(context, '/breakdown'),
                      ),
                    ],
                  ),
                ),
    );
  }
}
