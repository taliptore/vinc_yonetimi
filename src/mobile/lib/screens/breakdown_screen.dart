import 'package:flutter/material.dart';
import '../services/api_service.dart';

class BreakdownScreen extends StatefulWidget {
  const BreakdownScreen({super.key});

  @override
  State<BreakdownScreen> createState() => _BreakdownScreenState();
}

class _BreakdownScreenState extends State<BreakdownScreen> {
  final _api = ApiService();
  final _desc = TextEditingController();
  int? _craneId;
  bool _loading = false;
  String? _message;

  @override
  void dispose() {
    _desc.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_craneId == null) {
      setState(() => _message = 'Vinç seçin veya iş listesinden bir iş seçerek vinç bilgisi alın.');
      return;
    }
    setState(() => _loading = true);
    _message = null;
    try {
      await _api.reportBreakdown(_craneId!, _desc.text.trim());
      if (!mounted) return;
      setState(() => _message = 'Arıza bildirimi gönderildi.');
      _desc.clear();
    } catch (e) {
      if (mounted) setState(() => _message = e.toString().replaceFirst('Exception: ', ''));
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Arıza bildirimi')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const Text('Vinç ID (şimdilik manuel girin, iş ekranından da seçilebilir):'),
            const SizedBox(height: 8),
            TextField(
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Vinç ID',
                border: OutlineInputBorder(),
              ),
              onChanged: (v) => _craneId = int.tryParse(v),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _desc,
              maxLines: 4,
              decoration: const InputDecoration(
                labelText: 'Arıza açıklaması',
                border: OutlineInputBorder(),
              ),
            ),
            if (_message != null) Padding(padding: const EdgeInsets.only(top: 16), child: Text(_message!)),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: _loading ? null : _submit,
              child: Text(_loading ? 'Gönderiliyor...' : 'Gönder'),
            ),
          ],
        ),
      ),
    );
  }
}
