import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mobile/main.dart';

void main() {
  testWidgets('App starts and shows login or home', (WidgetTester tester) async {
    await tester.pumpWidget(const VincYonetimApp());
    await tester.pumpAndSettle(const Duration(seconds: 2));
    expect(find.byType(MaterialApp), findsOneWidget);
  });
}
