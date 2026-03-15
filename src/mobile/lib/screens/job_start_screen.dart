import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import '../services/api_service.dart';

class JobStartScreen extends StatefulWidget {
  const JobStartScreen({super.key, this.job});

  final Map<String, dynamic>? job;

  @override
  State<JobStartScreen> createState() => _JobStartScreenState();
}

class _JobStartScreenState extends State<JobStartScreen> {
  final _api = ApiService();
  bool _loading = false;
  String? _message;

  Future<Position?> _getLocation() async {
    final enabled = await Geolocator.isLocationServiceEnabled();
    if (!enabled) {
      setState(() => _message = 'Konum servisi kapalı.');
      return null;
    }
    var permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission == LocationPermission.denied) {
        setState(() => _message = 'Konum izni gerekli.');
        return null;
      }
    }
    return Geolocator.getCurrentPosition(desiredAccuracy: LocationAccuracy.high);
  }

  Future<void> _doStart() async {
    final job = widget.job;
    final id = job?['id'] as int?;
    if (id == null) return;
    setState(() => _loading = true);
    _message = null;
    try {
      final pos = await _getLocation();
      if (pos == null) { setState(() => _loading = false); return; }
      await _api.jobStart(id, pos.latitude, pos.longitude);
      if (!mounted) return;
      setState(() => _message = 'İş başlatıldı.');
    } catch (e) {
      if (mounted) setState(() => _message = e.toString().replaceFirst('Exception: ', ''));
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _doEnd() async {
    final job = widget.job;
    final id = job?['id'] as int?;
    if (id == null) return;
    setState(() => _loading = true);
    _message = null;
    try {
      final pos = await _getLocation();
      if (pos == null) { setState(() => _loading = false); return; }
      await _api.jobEnd(id, pos.latitude, pos.longitude);
      if (!mounted) return;
      setState(() => _message = 'İş bitirildi.');
      Navigator.of(context).pop();
    } catch (e) {
      if (mounted) setState(() => _message = e.toString().replaceFirst('Exception: ', ''));
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final job = widget.job ?? ModalRoute.of(context)?.settings.arguments as Map<String, dynamic>?;
    final site = job?['site'] is Map ? (job!['site'] as Map)['name'] : null;
    final crane = job?['crane'] is Map ? (job!['crane'] as Map)['plate'] : null;

    return Scaffold(
      appBar: AppBar(title: Text(site?.toString() ?? 'İş')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (site != null) Text('Şantiye: $site', style: Theme.of(context).textTheme.titleMedium),
            if (crane != null) Text('Vinç: $crane'),
            const SizedBox(height: 24),
            if (_message != null)
              Padding(
                padding: const EdgeInsets.only(bottom: 16),
                child: Text(_message!, style: TextStyle(color: _message!.startsWith('İş') ? Colors.green : Colors.red)),
              ),
            ElevatedButton.icon(
              onPressed: _loading ? null : _doStart,
              icon: _loading ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2)) : const Icon(Icons.play_arrow),
              label: const Text('İş başlat (GPS doğrula)'),
            ),
            const SizedBox(height: 12),
            ElevatedButton.icon(
              onPressed: _loading ? null : _doEnd,
              icon: const Icon(Icons.stop),
              label: const Text('İş bitir (GPS doğrula)'),
              style: ElevatedButton.styleFrom(backgroundColor: Colors.orange),
            ),
          ],
        ),
      ),
    );
  }
}
