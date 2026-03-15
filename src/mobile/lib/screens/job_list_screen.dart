import 'package:flutter/material.dart';
import '../services/api_service.dart';

class JobListScreen extends StatefulWidget {
  const JobListScreen({super.key});

  @override
  State<JobListScreen> createState() => _JobListScreenState();
}

class _JobListScreenState extends State<JobListScreen> {
  final _api = ApiService();
  List<dynamic> _jobs = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    try {
      final now = DateTime.now();
      final list = await _api.getJobs(from: now, to: now.add(const Duration(days: 365)));
      if (mounted) setState(() => _jobs = list);
    } catch (e) {
      if (mounted) setState(() => _error = e.toString().replaceFirst('Exception: ', ''));
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('İş listesi')),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(child: Text(_error!))
              : RefreshIndicator(
                  onRefresh: _load,
                  child: ListView.builder(
                    padding: const EdgeInsets.all(8),
                    itemCount: _jobs.length,
                    itemBuilder: (_, i) {
                      final j = _jobs[i] as Map<String, dynamic>;
                      final site = j['site'] is Map ? (j['site'] as Map)['name'] : null;
                      final crane = j['crane'] is Map ? (j['crane'] as Map)['plate'] : null;
                      final id = j['id'] as int?;
                      return Card(
                        child: ListTile(
                          title: Text(site?.toString() ?? 'İş'),
                          subtitle: Text('Vinç: ${crane ?? '-'}'),
                          trailing: const Icon(Icons.arrow_forward),
                          onTap: id != null ? () => Navigator.pushNamed(context, '/job_start', arguments: j) : null,
                        ),
                      );
                    },
                  ),
                ),
    );
  }
}
